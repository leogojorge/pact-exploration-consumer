using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PactExploration.Provider.Tests.Middleware;
using System.Reflection;

namespace PactExploration.Provider.Tests
{
    public class TestStartup
    {
        private Startup _proxy;

        public TestStartup(IConfiguration configuration)
        {
            _proxy = new Startup(configuration);
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddApplicationPart(Assembly.GetAssembly(typeof(Startup)));
            _proxy.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IActionDescriptorCollectionProvider actionProvider)
        {
            app.UseMiddleware<ProviderStateMiddleware>();
            _proxy.Configure(app, env, actionProvider);
        }
    }
}
