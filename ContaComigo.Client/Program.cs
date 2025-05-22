using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Globalization; // Necessário para CultureInfo
using System.Threading;     // Necessário para CultureInfo

// Adicionando o namespace do seu próprio projeto cliente.
// Isso garante que a classe 'App' e outros componentes sejam encontrados.
using ContaComigo.Client;

var culture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Esta linha adiciona o HttpClient ao contêiner de injeção de dependência.
// O BaseAddress é definido para a base do ambiente de hospedagem, permitindo chamadas à API.
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();