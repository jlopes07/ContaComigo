using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ContaComigo.Client;
using ContaComigo.Client.Services;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app"); // Garante que o App.razor seja o componente raiz
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configura o HttpClient apontando para a URL da API (ContaComigo)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7204")
});

// Registra o serviço que fará chamadas HTTP para a API
builder.Services.AddScoped<TransacaoService>();

await builder.Build().RunAsync();
