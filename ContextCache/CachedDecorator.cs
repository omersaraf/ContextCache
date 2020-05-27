using System;
using System.Threading.Tasks;

namespace ContextCache
{
    public static class CachedDecorator
    {
        public static async Task<TObject> Cached<TObject, TCacheContext>(this ContextCacheService service, string key, Func<TObject> func, TCacheContext context=default) where TObject: class where TCacheContext:ICacheContext
        {
            if (await service.Contains<TObject, TCacheContext>(key, context))
            {
                return await service.Get<TObject, TCacheContext>(key, context);
            }
            
            var result = func.Invoke();
            service.Set(key, result, context);
            return result;
        }
    }
}