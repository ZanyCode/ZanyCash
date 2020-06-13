using EventStore.ClientAPI;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.FSharp.Collections;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using ZanyCash.Core;
using ZanyStreams;
using static ZanyCash.Core.Types;
using static ZanyCash.Core.Types.Command;

namespace ZanyCash
{
    public class CoreAdapter
    {
        public IObservable<FSharpList<Transaction>> Transactions { get; }
        public IObservable<FSharpList<DayLiquidity>> Liquidity { get; }

        private Subject<Command> commands = new Subject<Command>();
        private BehaviorSubject<(DateTime, DateTime)> liquidityDateRange = 
            new BehaviorSubject<(DateTime, DateTime)>((new DateTime(2000,1,1), new DateTime(2030,1,1)));

        private readonly IEventStore eventStore;
        private string streamName;

        public CoreAdapter(IEventStore eventStore, IScopeProvider scopeProvider)
        {
            var userId = scopeProvider.GetScopeName();
            this.streamName = $"zanycash-{userId}";
            this.eventStore = eventStore;

            var sideEffectEvents = new Subject<Event>();
            var events = commands
                .Select(c => TransactionWriteModel.HandleCommand(c).Concat(LiquidityWriteModel.HandleCommand(c)))
                .Select(events => events.ToObservable())
                .Switch()
                .Merge(sideEffectEvents);

            
            var transactionReadState = events.Scan(TransactionReadModel.GetInitialState(), TransactionReadModel.HandleEvent).ReplayConnect(1);
            var transactionWriteState = events.Scan(TransactionWriteModel.GetInitialState(), (state, evt) =>
            {
                var (newState, newEvent) = TransactionWriteModel.HandleEvent(state, evt);
                if (newEvent != null)
                    sideEffectEvents.OnNext(newEvent.Value);

                return newState;
            }).ReplayConnect(1);

            var liquidityReadState = events.Scan(LiquidityReadModel.GetInitialState(), LiquidityReadModel.HandleEvent).ReplayConnect(1);

            var liquidityWriteState = events.Scan(LiquidityWriteModel.GetInitialState(), (state, evt) =>
            {
                var (newState, newEvent) = LiquidityWriteModel.HandleEvent(state, evt);
                if (newEvent != null)
                    sideEffectEvents.OnNext(newEvent.Value);

                return newState;
            }).ReplayConnect(1);


            var (pastEvents, futureEvents) = this.eventStore.Subscribe(this.streamName);
            foreach (var e in pastEvents)
                this.commands.OnNext(JsonConvert.DeserializeObject<Command>(e));

            futureEvents.Subscribe(e => this.commands.OnNext(JsonConvert.DeserializeObject<Command>(e)));
   
            this.Transactions = transactionReadState.Select(TransactionReadModel.GetTransactions);
            this.Liquidity = liquidityReadState.CombineLatest(liquidityDateRange, (state, dateRange) =>
            {
                var (startDate, endDate) = dateRange;
                return LiquidityReadModel.QueryLiquidityWithinDaterange(startDate, endDate, state);
            });
        }

        public void RunCommand(Command c)
        {
            var data = JsonConvert.SerializeObject(c);
            eventStore.Publish(this.streamName, data);
        }

        public void SetLiquidityDateRange(DateTime startDate, DateTime endDate)
        {
            this.liquidityDateRange.OnNext((startDate, endDate));
        }
    }

    public static class CoreExtensions
    {
        public static IObservable<T> ReplayConnect<T>(this IObservable<T> @this, int count)
        {
            var cObs = @this.Replay(count);
            cObs.Connect();
            return cObs;
        }
    }
}
