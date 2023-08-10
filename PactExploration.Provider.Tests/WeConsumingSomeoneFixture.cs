using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace PactExploration.Provider.Tests
{
    public class WeConsumingSomeoneFixture : IDisposable
    {
        private readonly IHost server;
        public Uri ServerUri { get; }

        public WeConsumingSomeoneFixture()
        {
            ServerUri = new Uri("http://localhost:5073");
            server = Host.CreateDefaultBuilder()
                         .ConfigureWebHostDefaults(webBuilder =>
                         {
                             webBuilder.UseUrls(ServerUri.ToString());
                             webBuilder.UseStartup<TestStartup>();
                         })
                         .Build();
            server.Start();
        }

        public void Dispose()
        {
            server.Dispose();
        }
    }
}