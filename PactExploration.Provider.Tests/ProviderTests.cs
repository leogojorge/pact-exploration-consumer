using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactExploration.Provider.Tests;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Native;
using Xunit.Abstractions;

namespace Provider.Tests
{
    public class ProviderTests //: IClassFixture<WeConsumingSomeoneFixture>
    {
        private string pactServiceUri = "http://127.0.0.1:9001";
        private string serviceUri = "http://127.0.0.1:5073";
        private ITestOutputHelper outputHelper;
        //private WeConsumingSomeoneFixture weConsumingSomeoneFixture;


        public ProviderTests(ITestOutputHelper output)//, WeConsumingSomeoneFixture weConsumingSomeoneFixture)
        {
            this.outputHelper = output;
            //this.weConsumingSomeoneFixture = weConsumingSomeoneFixture;
        }

        [Fact]
        public void ValidateWeConsumingSomeonePact()
        {
            var config = new PactVerifierConfig
            {
                Outputters = new List<IOutput> { new XUnitOutput(this.outputHelper) }//Xunit n consegue ler o erro q o PactNet loga. é necessário ter um custom só pra fazer o WriteLine dos erros.
            };

            //esse webHost sobe a aplicação pra executar os testes. tem casos q é possível rodar teste no provedor sem subir a aplicação, outros não. não entendi ainda quando usar uma maneira ou outra.
            using (var webHost = WebHost.CreateDefaultBuilder().UseStartup<TestStartup>().UseUrls(this.pactServiceUri).Build())
            {
                webHost.Start();

                var pactFile = new FileInfo(Path.Join("..", "..", "..", "..", "..", "..", "consumer", "pacts", "WeConsumingSomeone-SomeProvider.json"));
                var pactOptions = new PactUriOptions("faM71GPVLZkuKYPcRMYo2g");

                IPactVerifier pactVerifier = new PactVerifier(config);
                pactVerifier
                      //.FromPactBroker(new Uri("https://stonepagamentos.pactflow.io"), pactOptions)
                      .FromPactFile(pactFile)
                      .WithProviderStateUrl(new Uri($"{pactServiceUri}/provider-states"))
                      .ServiceProvider("SomeProvider", new Uri(pactServiceUri))//tem q ser a url do serviço q vai estar up pra poder rodar as validações em cima
                      .HonoursPactWith("WeConsumingSomeone")//não entendi, o parâmetro é consumerName, mas só funciona se eu passar o nome do provedor
                      .Verify();

                webHost.StopAsync();
            }
        }
    }

    /*
     * versão do gringo do stack overflow
     public class ProviderTests : IDisposable
    {
        private readonly IHost server;
        public Uri ServerUri { get; }
        private readonly PactVerifier verifier;

        public ProviderTests(ITestOutputHelper output)
        {
            this.outputHelper = output;
            ServerUri = new Uri("http://localhost:5037");
            server = Host.CreateDefaultBuilder()
                         .ConfigureWebHostDefaults(webBuilder =>
                         {
                             webBuilder.UseUrls(ServerUri.ToString());
                             webBuilder.UseStartup<TestStartup>();
                         })
                         .Build();
            server.Start();

            this.verifier = new PactVerifier(new PactVerifierConfig
            {
                Outputters = new List<IOutput> { new XUnitOutput(this.outputHelper) }//Xunit n consegue ler o erro q o PactNet loga. é necessário ter um custom só pra fazer o WriteLine dos erros.
            });
        }

        [Fact]
        public void ValidateWeConsumingSomeonePact()
        {
                var pactFile = new FileInfo(Path.Join("..", "..", "..", "..", "..", "..", "consumer", "pacts", "WeConsumingSomeone-SomeProvider.json"));
                var pactOptions = new PactUriOptions("faM71GPVLZkuKYPcRMYo2g");

                this.verifier
                    .FromPactBroker(new Uri("https://stonepagamentos.pactflow.io"), pactOptions)
                    //.FromPactFile(pactFile)
                    .WithProviderStateUrl(new Uri($"{pactServiceUri}/provider-states"))
                    .ServiceProvider("SomeProvider", new Uri(pactServiceUri))
                    .HonoursPactWith("WeConsumingSomeone")//não entendi, o parâmetro é consumerName, mas só funciona se eu passar o nome do provedor
                    .Verify();

        }

        


        public void Dispose()
        {
            server.Dispose();
        }
    }
     */
}