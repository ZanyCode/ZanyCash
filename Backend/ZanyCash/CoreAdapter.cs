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

        private readonly IEventStore eventStore;

        public CoreAdapter(IEventStore eventStore)
        {
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

            int eventCount = 0;
            var liquidityWriteState = events.Scan(LiquidityWriteModel.GetInitialState(), (state, evt) =>
            {
                Console.WriteLine(eventCount);
                var (newState, newEvent) = LiquidityWriteModel.HandleEvent(state, evt);
                if (newEvent != null)
                    sideEffectEvents.OnNext(newEvent.Value);

                return newState;
            }).ReplayConnect(1);

            var (pastEvents, futureEvents) = this.eventStore.Subscribe("zanycash");
            foreach (var e in pastEvents)
                this.commands.OnNext(JsonConvert.DeserializeObject<Command>(e));

            futureEvents.Subscribe(e => this.commands.OnNext(JsonConvert.DeserializeObject<Command>(e)));

            //connection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));
            //connection.ConnectAsync().Wait();

            //// Load initial events
            //StreamEventsSlice currentSlice;
            //long nextSliceStart = StreamPosition.Start;
            //do
            //{
            //    currentSlice =
            //    connection.ReadStreamEventsForwardAsync("zanycash", nextSliceStart, 200, false).Result;
            //    nextSliceStart = currentSlice.NextEventNumber;
            //    foreach (var evt in currentSlice.Events)
            //    {
            //        var command = JsonConvert.DeserializeObject<Command>(Encoding.UTF8.GetString(evt.Event.Data));
            //        this.commands.OnNext(command);
            //        eventCount++;
            //        Console.WriteLine("EventCount: " + eventCount);
            //    }
            //} while (!currentSlice.IsEndOfStream);

            //connection.SubscribeToStreamFrom("zanycash", eventCount-1, CatchUpSubscriptionSettings.Default, (_, x) =>
            //{
            //    var command = JsonConvert.DeserializeObject<Command>(Encoding.UTF8.GetString(x.Event.Data));
            //    this.commands.OnNext(command);
            //});

            var t = transactionReadState.Select(TransactionReadModel.GetTransactions);
            this.Transactions = t;
        }

        public void RunCommand(Command c)
        {
            var streamName = "zanycash";
            var data = JsonConvert.SerializeObject(c);
            eventStore.Publish(streamName, data);
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
