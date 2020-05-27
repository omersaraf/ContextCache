# ContextCache
C# Library for caching objects and operations based on context (request, session, tenant, global and etc)

# Use case

Let's say you have big project in which you use many services, any of them may call some heavy operation (database query/computation/etc) and because of services isolation you may execute the same query multiple times.

The simplest example come to mind - get user by id from DB. You may query the user multiple times in single request, for authorization, for personal information, for personalized data and etc.

# Usage
If you want to use request level cache you can call it this way
```C#
public class MyClass 
{
    ... // props definition
    
    public MyClass(ContextCacheService contextCacheService, IUserProvider userProvider){
        _contextCacheService = contextCacheService;
        _userProvider = userProvider;
    }

    public User GetUserById(string userId){
        return _contextCacheService.Cached<User, RequestLevelCacheContext>(userId, () => _userProvider.Provide(userId));
    }
}
```

This way if no matter how many times you'd call `GetUserById` **in the same request** it'll be cached after the first time.

# Add new context
If you want to add your own context (let's say *tenant* based), all You have to do is to implement `ICacheContext` and `ICacheProvider` (if you want easier implementation you can implement GenericCacheProvider) and register your new CacheProvider into the container (or pass it manually in the ContextCacheService constructor)

Context:
```C#
public class TenantContext : ICacheContext
{
    public string TenantId { get; }

    public TenantContext(string tenantId)
    {
        TenantId = tenantId;
    }
}
```

CacheProvider:
```C#
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
```
*(Here I used ICacheService as abstraction for caching, but it could be your own implementation such as memory cache/redis/memcache/etc)*

# Code reference
In this repository you can use the ContextCache.Main project for usage examples which uses RequestLevel, Session and Tenant context caches.

In the example there is "heavy" (sleep for 1 second) operation, which being executes 10 times meaning without cache you will have to wait 10 seconds.
* **RequestLevelCache** - will execute the operation only one time per request
* **SessionLevelCache** - will execute the operation one time per session (meaning in some requests the operation won't be execute at all)
* **TenantCache** - will execute the operation one time per url parameter *tenant* (meaning some users may not have to wait for the opeation to being execute at all because it's been already executed in the same *tenant*)

```C#
public class CacheController : Controller
{
    private readonly ContextCacheService _contextCacheService;

    public CacheController(ContextCacheService contextCacheService, ILogger<CacheController> logger)
    {
        _contextCacheService = contextCacheService;
    }

    private static TestObject MySlowFunction()
    {
        Thread.Sleep(1000);
        return new TestObject();
    }

    private async Task<int> ExecuteCache<T>(T context) where T : ICacheContext
    {
        var numberOfCalls = 0;
        for (var i = 0; i < 10; i++)
        {
            await _contextCacheService.Cached("test", () =>
            {
                numberOfCalls++;
                return MySlowFunction();
            }, context);
        }

        return numberOfCalls;
    }

    [Route("tenant")]
    public Task<int> Tenant(string tenant)
    {
        return ExecuteCache(new TenantContext(tenant));
    }
    
    [Route("request")]
    public Task<int> RequestLevel()
    {
        return ExecuteCache(new RequestLevelCacheContext());
    }
    
    [Route("session")]
    public Task<int> Session()
    {
        return ExecuteCache(new SessionLevelCacheContext());
    }
}
```
