using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using PactExploration.Provider.Tests.Middleware;

namespace PactExploration.Provider.Tests
{
    public class TestStartup
    {
        private Startup _proxy;

        public TestStartup()
        {
            _proxy = new Startup();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _proxy.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ProviderStateMiddleware>();
            _proxy.Configure(app, env);
        }
    }
}
