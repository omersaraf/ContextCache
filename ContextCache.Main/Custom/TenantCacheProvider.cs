using System.Threading.Tasks;

namespace ContextCache.Main.Custom
{
    public class TenantCacheProvider : GenericCacheProvider<TenantContext>
    {
        private readonly ICacheService _cacheService;
        
        public TenantCacheProvider(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        private static string ComposeKey(string key, TenantContext context)
        {
            return $"{context.TenantId}_{key}";
        }

        protected override Task<bool> ContainsWithContext<T>(string key, TenantContext context)
        {
            return _cacheService.Contains<T>(ComposeKey(key, context));
        }

        protected override Task<T> GetWithContext<T>(string key, TenantContext context)
        {
            return _cacheService.Get<T>(ComposeKey(key, context));
        }

        protected override void SetWithContext<T>(string key, T value, TenantContext context)
        {
            _cacheService.Set(ComposeKey(key, context), value);
        }
    }
}