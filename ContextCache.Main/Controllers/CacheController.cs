using System.Threading;
using System.Threading.Tasks;
using ContextCache.HttpCache.RequestLevel;
using ContextCache.HttpCache.Session;
using ContextCache.Main.Custom;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ContextCache.Main.Controllers
{
    [Route("cache")]
    public class CacheController : Controller
    {
        private readonly ContextCacheService _contextCacheService;
        private readonly ILogger _logger;

        public CacheController(ContextCacheService contextCacheService, ILogger<CacheController> logger)
        {
            _contextCacheService = contextCacheService;
            this._logger = logger;
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
                    _logger.Log(LogLevel.Debug, $"Executed {context}");
                    numberOfCalls++;
                    return MySlowFunction();
                }, context);
                _logger.Log(LogLevel.Debug, $"Iteration {context}");
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
    
    public class TestObject
    {
        public int Integer { get; set; }
    }
}