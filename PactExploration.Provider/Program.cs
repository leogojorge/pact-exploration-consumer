
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;

namespace PactExploration.Provider
{
    public class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                        .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);
            services.AddEndpointsApiExplorer();
            services.AddSingleton<FakeDatabase>();
            services.AddSwaggerGen();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IActionDescriptorCollectionProvider actionProvider)
        {
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(e => e.MapControllers());
            app.UseHttpsRedirection();

            var routes = actionProvider.ActionDescriptors.Items.Where(x => x.AttributeRouteInfo != null);
            foreach (var route in routes)
            {
                Console.WriteLine($"{route.AttributeRouteInfo.Template}");
            }

            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }

    }
}