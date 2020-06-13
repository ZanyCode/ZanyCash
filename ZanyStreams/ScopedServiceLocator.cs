using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZanyStreams
{
    public interface IScopedServiceLocator
    {
        T GetRequiredService<T>(string scopeName);
        IEnumerable<T> GetServices<T>(string scopeName);
    }

    public class ScopedServiceLocator : IScopedServiceLocator
    {
        private readonly IServiceProvider rootProvider;
        private Dictionary<string, IServiceProvider> scopedProviders = new Dictionary<string, IServiceProvider>();
        private object lockObject = new object();

        public ScopedServiceLocator(IServiceProvider provider)
        {
            this.rootProvider = provider;
        }

        public T GetRequiredService<T>(string scopeName)
        {
            lock (lockObject)
            {
                EnsureScopeExists(scopeName);
                return scopedProviders[scopeName].GetRequiredService<T>();
            }
        }

        public IEnumerable<T> GetServices<T>(string scopeName)
        {
            lock (lockObject)
            {
                EnsureScopeExists(scopeName);
                return scopedProviders[scopeName].GetServices<T>();
            }
        }

        private void EnsureScopeExists(string scopeName)
        {
            if (!scopedProviders.ContainsKey(scopeName))
            {
                var provider = rootProvider.CreateScope().ServiceProvider;
                provider.GetRequiredService<IScopeProvider>().SetScopeName(scopeName);
                scopedProviders.Add(scopeName, provider);
            }
        }
    }
}
