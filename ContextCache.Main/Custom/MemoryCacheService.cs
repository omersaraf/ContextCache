using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContextCache.Main.Custom
{
    public class MemoryCacheService : ICacheService
    {
        private readonly Dictionary<string, object> _cache;

        public MemoryCacheService()
        {
            _cache = new Dictionary<string, object>();            
        }
        
        public Task<bool> Contains<T>(string key)
        {
            return Task.Run(() => _cache.ContainsKey(key));
        }

        public Task<T> Get<T>(string key)
        {
            return Task.Run(() => (T) _cache[key]);
        }

        public void Set<T>(string key, T obj)
        {
            Task.Run(() => _cache[key] = obj);
        }
    }
}