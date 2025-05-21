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
using System.Net.Http;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddHttpClient();
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        builder.Services.AddSingleton<ITransacaoRepository, InMemoryTransacaoRepository>();

        builder.Services.AddTransient<RegistrarTransacao>();
        builder.Services.AddTransient<ObterTodasTransacoes>();

        var app = builder.Build();

        // **INÍCIO DO BLOCO DE SEEDING - REMOVIDA A CONDIÇÃO IF**
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var transacaoRepository = services.GetRequiredService<ITransacaoRepository>();

            // Adicione os dados diretamente, sem verificação.
            transacaoRepository.Adicionar(new Transacao("Aluguel Mensal", 1800.00m, new DateTime(2024, 5, 1)));
            transacaoRepository.Adicionar(new Transacao("Salário", 5000.00m, new DateTime(2024, 5, 5)));
            transacaoRepository.Adicionar(new Transacao("Conta de Luz", 150.75m, new DateTime(2024, 5, 10)));
            transacaoRepository.Adicionar(new Transacao("Compras Supermercado", 320.50m, new DateTime(2024, 5, 15)));
        }
        // **FIM DO BLOCO DE SEEDING**

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

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(ContaComigo.Client._Imports).Assembly);

        app.MapControllers();

        app.Run();
    }
}