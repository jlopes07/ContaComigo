// ContaComigo.Tests/CustomWebApplicationFactory.cs

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using ContaComigo; // **Este using é crucial para referenciar sua classe 'Program' da aplicação principal**

namespace ContaComigo.Tests;

// TProgram deve ser a classe Program da sua aplicação, e não Microsoft.VisualStudio.TestPlatform.TestHost.Program
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        // ... outras configurações
    }
}