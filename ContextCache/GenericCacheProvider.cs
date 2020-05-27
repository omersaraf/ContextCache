using System.Threading.Tasks;

namespace ContextCache
{
    public abstract class GenericCacheProvider<TCacheContext> : ICacheProvider where TCacheContext : class, ICacheContext
    {
        public bool IsForContext<TKCacheContext>() where TKCacheContext : ICacheContext
        {
            return typeof(TKCacheContext) == typeof(TCacheContext);
        }

        public Task<T> Get<T>(string key, ICacheContext context) where T : class
        {
            return GetWithContext<T>(key, context as TCacheContext);
        }

        public void Set<T>(string key, T value, ICacheContext context) where T : class
        {
            SetWithContext(key, value, context as TCacheContext);
        }

        public Task<bool> Contains<T>(string key, ICacheContext context) where T : class
        {
            return ContainsWithContext<T>(key, context as TCacheContext);
        }


        protected abstract Task<bool> ContainsWithContext<T>(string key, TCacheContext context) where T : class;
        protected abstract Task<T> GetWithContext<T>(string key, TCacheContext context) where T : class;

        protected abstract void SetWithContext<T>(string key, T value, TCacheContext context) where T : class;
    }
}