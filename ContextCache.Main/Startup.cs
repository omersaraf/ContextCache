using System;
using ContextCache.HttpCache.RequestLevel;
using ContextCache.HttpCache.Session;
using ContextCache.Main.Custom;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ContextCache.Main
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            services.AddHttpContextAccessor();
            services.AddDistributedMemoryCache();
            services.AddSession();
            services.AddSingleton<ContextCacheService>();
            services.AddSingleton<ICacheProvider, RequestLevelCacheProvider>();
            services.AddSingleton<ICacheProvider, TenantCacheProvider>();
            services.AddSingleton<ICacheProvider, SessionLevelCacheProvider>();
            services.AddSingleton<ICacheService, MemoryCacheService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}