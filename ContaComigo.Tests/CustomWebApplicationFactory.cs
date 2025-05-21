// ContaComigo.Tests/CustomWebApplicationFactory.cs

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting; // Necessário para IHostBuilder
using ContaComigo; // **CRUCIAL**: Este using é necessário para referenciar sua classe 'Program'

namespace ContaComigo.Tests
{
    // Esta classe herda de WebApplicationFactory e é o ponto de entrada
    // para configurar o ambiente de teste da sua aplicação ASP.NET Core.
    // 'Program' é a classe de inicialização da sua aplicação principal (ContaComigo).
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        // Sobrescreve este método para configurar o host da web da sua aplicação.
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Você pode usar isso para configurar o ambiente (e.g., "Development")
            // para que a sua aplicação em teste se comporte como em desenvolvimento.
            builder.UseEnvironment("Development");

            // Quaisquer outras configurações gerais para o ambiente de teste podem ir aqui.
            // Por exemplo, substituição de serviços ou configurações de log.
            // No entanto, as substituições específicas do repositório (mock)
            // já estão no seu TransacoesControllerIntegrationTests.cs, então não precisamos duplicar.
        }
    }
}