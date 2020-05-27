using System.Threading.Tasks;

namespace ContextCache.Main.Custom
{
    public interface ICacheService
    {
        Task<bool> Contains<T>(string key);

        Task<T> Get<T>(string key);

        void Set<T>(string key, T obj);
    }
}