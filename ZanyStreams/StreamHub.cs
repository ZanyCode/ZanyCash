using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ZanyStreams
{
    [Authorize]
    public class StreamHub : Hub
    {
        private readonly IHubContext<StreamHub> context;
        private readonly IScopedServiceLocator serviceLocator;

        public StreamHub(IHubContext<StreamHub> context, IScopedServiceLocator serviceLocator)
        {
            this.context = context;
            this.serviceLocator = serviceLocator;
        }

        public void ConnectToStream(string streamName)
        {
            var connectionId = this.Context.ConnectionId;
            var userId = this.Context.UserIdentifier;
            var stream = GetStream(userId, streamName);
            stream.Subscribe(x => this.context.Clients.Client(connectionId).SendAsync(streamName, x));
        }

        public string GetConnectionId()
        {
            return this.Context.ConnectionId;
        }

        private IObservable<Result> GetStream(string scopeName, string streamName)
        {
            var publishers = this.serviceLocator.GetServices<IStreamPublisher>(scopeName);
            var allStreams = publishers.SelectMany(p => p.GetStreams());
            try
            {
                var stream = allStreams.Single(s => s.Name.Equals(streamName));
                var obs = stream
                    .GetData()
                    .Select(x => new Result(x))
                    .Catch<Result, Exception>(e => Observable.Return(new Result(e)));

                return obs;
            }
            catch(Exception e)
            {
                return Observable.Return(new Result(new Error(
                    $"Error connecting to stream: Either no stream or multiple streams with name {streamName} exist. Original Error: {e.Message}")));                
            }
        }
    }
}
