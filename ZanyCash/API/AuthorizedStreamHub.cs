using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZanyStreams;

namespace ZanyCash.API
{
    public class AuthorizedStreamHub : StreamHub
    {
        public AuthorizedStreamHub(IHubContext<StreamHub> context, IScopedServiceLocator serviceLocator) : base(context, serviceLocator)
        {
        }
    }
}
