using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using PactExploration.Provider;
using PactExploration.Provider.Tests;
using PactNet;
using PactNet.Infrastructure.Outputters;
using PactNet.Native;
using Xunit.Abstractions;

namespace Provider.Tests
{
    public class ProviderTests
    {
        private string pactServiceUri = "http://127.0.0.1:9011";

        private ITestOutputHelper outputHelper;

        public ProviderTests(ITestOutputHelper output)
        {
            this.outputHelper = output;
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
                    .FromPactBroker(new Uri("https://stonepagamentos.pactflow.io"), pactOptions)
                    //.FromPactFile(pactFile)
                    .WithProviderStateUrl(new Uri($"{pactServiceUri}/provider-states"))
                    .ServiceProvider("SomeProvider", new Uri(pactServiceUri))
                    .HonoursPactWith("SomeProvider")//n�o entendi, o par�metro � consumerName, mas s� funciona se eu passar o nome do provedor

                    .Verify();

                webHost.StopAsync();
            }
        }
    }
}