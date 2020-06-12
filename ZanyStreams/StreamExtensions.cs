using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZanyStreams
{
    public static class StreamExtensions
    {
        public static IServiceCollection AddStreams(this IServiceCollection services)
        {
            services.AddSignalR();

            services.AddSingleton<IScopedServiceLocator, ScopedServiceLocator>();
            return services;
        }

        public static Stream ToStream<T>(this IObservable<T> @this, string name)
        {
            return new Stream(name, () => @this as IObservable<object>);
        }
    }
}
