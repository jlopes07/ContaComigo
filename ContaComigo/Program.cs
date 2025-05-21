using ContaComigo.Client.Pages;
using ContaComigo.Components;
using ContaComigo.Application.Interfaces;
using ContaComigo.Application.UseCases.Transacoes;
using ContaComigo.Infrastructure.Repositories;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Registrar Repositório e Casos de Uso
builder.Services.AddSingleton<ITransacaoRepository, InMemoryTransacaoRepository>();
builder.Services.AddTransient<RegistrarTransacao>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ContaComigo.Client._Imports).Assembly);

app.Run();
