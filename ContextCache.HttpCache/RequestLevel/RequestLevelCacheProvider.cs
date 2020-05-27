using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ContextCache.HttpCache.RequestLevel
{
    public class RequestLevelCacheProvider : GenericCacheProvider<RequestLevelCacheContext>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestLevelCacheProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private static string ComposeKey(string key)
        {
            return $"requestLevelCache_{key}";
        }

        protected override Task<bool> ContainsWithContext<T>(string key, RequestLevelCacheContext context)
        {
            return Task.FromResult(_httpContextAccessor.HttpContext.Items.ContainsKey(ComposeKey(key))); 
        }

        protected override Task<T> GetWithContext<T>(string key, RequestLevelCacheContext context)
        {
            return Task.FromResult(_httpContextAccessor.HttpContext.Items[ComposeKey(key)] as T);
        }

        protected override void SetWithContext<T>(string key, T value, RequestLevelCacheContext context)
        {
            _httpContextAccessor.HttpContext.Items[ComposeKey(key)] = value;
        }
    }
}