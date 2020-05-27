using System.Threading.Tasks;

namespace ContextCache
{
    public interface ICacheProvider
    {
        public bool IsForContext<TCacheContext>() where TCacheContext : ICacheContext;

        public abstract Task<T> Get<T>(string key, ICacheContext context) where T : class;

        public abstract void Set<T>(string key, T value, ICacheContext context) where T : class;

        public abstract Task<bool> Contains<T>(string key, ICacheContext context) where T : class;
    }
}