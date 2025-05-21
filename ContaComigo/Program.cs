// ContaComigo/Program.cs

using ContaComigo.Client.Pages;
using ContaComigo.Components;
using ContaComigo.Application.Interfaces;
using ContaComigo.Application.UseCases.Transacoes;
using ContaComigo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using ContaComigo.Shared.Models;

// Remove o 'namespace ContaComigo' explícito para o modelo de hospedagem mínimo,
// ou mantenha-o se preferir o estilo de classe Program explícita.
// Se você quer ter uma classe Program explícita e *não* o modelo mínimo,
// o código abaixo já estará no formato correto para o WebApplicationFactory.

// O código global (sem namespace) da Program.cs do .NET 6+ implica
// que a classe Program é criada automaticamente como 'internal static partial class Program'.
// Se você está usando o modelo explícito (como o seu), precisa que ela seja 'public partial class'.

// Certifique-se que esta declaração 'partial' está FORA do método Main.
// Se você estiver usando o modelo de topo do arquivo (sem public class Program e Main),
// o compilador gerencia o 'partial' automaticamente.
// Se você está explicitamente declarando public class Program e Main,
// então a declaração 'public partial class Program { }' ao final do arquivo é crucial.

var builder = WebApplication.CreateBuilder(args);

// Suporte a controladores (necessário para APIs)
builder.Services.AddControllers();

// Suporte ao Blazor Server + WASM híbrido
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Registrar Repositório e Casos de Uso
builder.Services.AddSingleton<ITransacaoRepository, InMemoryTransacaoRepository>();
builder.Services.AddTransient<RegistrarTransacao>();
builder.Services.AddTransient<ObterTodasTransacoes>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Mapeia os componentes Blazor
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ContaComigo.Client._Imports).Assembly);

// Mapeia os controladores de API
app.MapControllers();

app.Run();

// Esta linha é CRUCIAL para que WebApplicationFactory<Program> funcione
// quando você tem um arquivo Program.cs no estilo "old-school" com Main.
// Ela precisa estar FORA de qualquer classe, no final do arquivo Program.cs.
// Se você está usando o modelo de topo de arquivo do .NET 6+, você não precisa disso.
// Mas como você tem 'public class Program { public static void Main ... }',
// você precisa desta declaração 'partial' externa.
public partial class Program { }