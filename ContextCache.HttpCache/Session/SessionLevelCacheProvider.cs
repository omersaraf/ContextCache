using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ContextCache.HttpCache.Session
{
    public class SessionLevelCacheProvider : GenericCacheProvider<SessionLevelCacheContext>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionLevelCacheProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task<bool> ContainsWithContext<T>(string key, SessionLevelCacheContext context)
        {
            return Task.FromResult(_httpContextAccessor.HttpContext.Session.IsAvailable &&
                   _httpContextAccessor.HttpContext.Session.Get<T>(key) != default(T));
        }

        protected override Task<T> GetWithContext<T>(string key, SessionLevelCacheContext context)
        {
            return Task.FromResult(_httpContextAccessor.HttpContext.Session.Get<T>(key));
        }

        protected override void SetWithContext<T>(string key, T value, SessionLevelCacheContext context)
        {
            _httpContextAccessor.HttpContext.Session.Set(key, value);
        }
    }
}
