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
                Outputters = new List<IOutput> { new XUnitOutput(this.outputHelper) }//Xunit n consegue ler o erro q o PactNet loga. � necess�rio ter um custom s� pra fazer o WriteLine dos erros.
            };

            //esse webHost sobe a aplica��o pra executar os testes. tem casos q � poss�vel rodar teste no provedor sem subir a aplica��o, outros n�o. n�o entendi ainda quando usar uma maneira ou outra.
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
                      .ServiceProvider("SomeProvider", new Uri(pactServiceUri))//tem q ser a url do servi�o q vai estar up pra poder rodar as valida��es em cima
                      .HonoursPactWith("WeConsumingSomeone")//n�o entendi, o par�metro � consumerName, mas s� funciona se eu passar o nome do provedor
                      .Verify();

                webHost.StopAsync();
            }
        }
    }

    /*
     * vers�o do gringo do stack overflow
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
                Outputters = new List<IOutput> { new XUnitOutput(this.outputHelper) }//Xunit n consegue ler o erro q o PactNet loga. � necess�rio ter um custom s� pra fazer o WriteLine dos erros.
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
                    .HonoursPactWith("WeConsumingSomeone")//n�o entendi, o par�metro � consumerName, mas s� funciona se eu passar o nome do provedor
                    .Verify();

        }

        


        public void Dispose()
        {
            server.Dispose();
        }
    }
     */
}