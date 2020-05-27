using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContextCache
{
    public class ContextCacheService
    {
        private readonly IEnumerable<ICacheProvider> _providers;

        public ContextCacheService(IEnumerable<ICacheProvider> providers)
        {
            _providers = providers;
        }

        private ICacheProvider GetCacheProviderForContext<TCacheContext>() where TCacheContext : ICacheContext
        {
            var provider = _providers.FirstOrDefault(p => p.IsForContext<TCacheContext>());
            if (provider == null)
            {
                throw new InvalidOperationException($"Could not find cache provider for context {typeof(TCacheContext).Name}, did you register the provider in the container?");
            }

            return provider;
        }
        
        public Task<T> Get<T, TCacheContext>(string key, TCacheContext cacheContext = default) where T : class where TCacheContext : ICacheContext
        {
            var provider = GetCacheProviderForContext<TCacheContext>();
            return provider.Get<T>(key, cacheContext);
        }
        
        public void Set<T, TCacheContext>(string key, T value, TCacheContext cacheContext) where T : class where TCacheContext : ICacheContext
        {
            var provider = GetCacheProviderForContext<TCacheContext>();
            provider.Set(key, value, cacheContext);
        }

        public Task<bool> Contains<T, TCacheContext>(string key, TCacheContext context) where T : class where TCacheContext : ICacheContext
        {
            var provider = GetCacheProviderForContext<TCacheContext>();
            return provider.Contains<T>(key, context);
        }
    }
}