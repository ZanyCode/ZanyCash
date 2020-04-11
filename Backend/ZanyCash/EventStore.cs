using EventStore.ClientAPI;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZanyCash
{
    public interface IEventStore
    {
        Task Connect();
        (IEnumerable<string>, IObservable<string>) Subscribe(string eventStream);

        void Publish(string eventStream, string data);

    }

    public class EventStore : IEventStore
    {
        private IEventStoreConnection connection;

        public async Task Connect()
        {
            connection = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));
            await connection.ConnectAsync();
        }

        public (IEnumerable<string>, IObservable<string>) Subscribe(string eventStream)
        {
            // Load initial events
            int eventCount = 0;
            var pastEvents = new List<string>();
            StreamEventsSlice currentSlice;
            long nextSliceStart = StreamPosition.Start;
            do
            {
                currentSlice =
                connection.ReadStreamEventsForwardAsync("zanycash", nextSliceStart, 200, false).Result;
                nextSliceStart = currentSlice.NextEventNumber;
                foreach (var evt in currentSlice.Events)
                {
                    pastEvents.Add(Encoding.UTF8.GetString(evt.Event.Data));
                    eventCount++;
                }
            } while (!currentSlice.IsEndOfStream);

            // Subscribe to Future events
            var futureEvents = Observable.Create<string>(observer =>
            {
                var subscription = connection.SubscribeToStreamFrom("zanycash", eventCount - 1, CatchUpSubscriptionSettings.Default, (_, x) =>
                {
                    observer.OnNext(Encoding.UTF8.GetString(x.Event.Data));
                });

                return () => subscription.Stop();
            });

            return (pastEvents, futureEvents);            
        }

        public void Publish(string eventStream, string data)
        {
            var metadata = "{}";
            var eventType = "command";
            var eventPayload = new EventData(Guid.NewGuid(), eventType, true, Encoding.UTF8.GetBytes(data), Encoding.UTF8.GetBytes(metadata));
            connection.AppendToStreamAsync(eventStream, ExpectedVersion.Any, eventPayload).Wait();
        }
    }

    public static class EventStoreExtensions
    {
        public static IApplicationBuilder UseEventStore(this IApplicationBuilder app)
        {
            app.Use(x =>
            {
                var eventStore = app.ApplicationServices.GetService(typeof(IEventStore)) as IEventStore;
                eventStore.Connect().Wait();
                return x;
            });

            return app;
        }
    }
}
