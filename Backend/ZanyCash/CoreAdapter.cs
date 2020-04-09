using EventStore.ClientAPI;
using Microsoft.FSharp.Collections;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using ZanyCash.Core;
using static ZanyCash.Core.Types;
using static ZanyCash.Core.Types.Command;

namespace ZanyCash
{
    public class CoreAdapter
    {
        public IObservable<FSharpList<Transaction>> Transactions { get; }

        private Subject<Command> commands = new Subject<Command>();

        private IEventStoreConnection connection;

        public CoreAdapter()
        {
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

            connection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));
            connection.ConnectAsync().Wait();

            // Load initial events
            StreamEventsSlice currentSlice;
            long nextSliceStart = StreamPosition.Start;
            int eventCount = 0;
            do
            {
                currentSlice =
                connection.ReadStreamEventsForwardAsync("zanycash", nextSliceStart, 200, false).Result;
                nextSliceStart = currentSlice.NextEventNumber;
                foreach (var evt in currentSlice.Events)
                {
                    var command = JsonConvert.DeserializeObject<Command>(Encoding.UTF8.GetString(evt.Event.Data));
                    this.commands.OnNext(command);
                    eventCount++;
                }
            } while (!currentSlice.IsEndOfStream);

            connection.SubscribeToStreamFrom("zanycash", eventCount-1, CatchUpSubscriptionSettings.Default, (_, x) =>
            {
                var command = JsonConvert.DeserializeObject<Command>(Encoding.UTF8.GetString(x.Event.Data));
                this.commands.OnNext(command);
            });

            var t = transactionReadState.Select(TransactionReadModel.GetTransactions);
            this.Transactions = t;
        }

        public void RunCommand(Command c)
        {
            var streamName = "zanycash";
            var eventType = "command";
            var data = JsonConvert.SerializeObject(c);
            var metadata = "{}";
            var eventPayload = new EventData(Guid.NewGuid(), eventType, true, Encoding.UTF8.GetBytes(data), Encoding.UTF8.GetBytes(metadata));
            connection.AppendToStreamAsync(streamName, ExpectedVersion.Any, eventPayload).Wait();
            // commands.OnNext(c);
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
