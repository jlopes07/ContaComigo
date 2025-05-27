using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using ContaComigo.Application.UseCases.Transacoes;
using ContaComigo.Application.Interfaces;
using ContaComigo.Infrastructure.Repositories;

namespace ContaComigo;

public class Program 
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>(); // <--- Usaremos a classe Startup
            });
}

// Nova classe Startup para configurar serviços e pipeline
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    // Este método configura os serviços da sua aplicação
    public void ConfigureServices(IServiceCollection services)
    {
        // Adiciona serviços ao contêiner.
        services.AddControllersWithViews(); // Necessário para seus controladores de API
        services.AddRazorPages(); // Necessário para o fallback do Blazor e se tiver Razor Pages

        // **Registro dos seus Use Cases e Repositórios**
        // Estes são os serviços que seus controladores usarão.
        services.AddScoped<RegistrarTransacao>();
        services.AddScoped<ObterTodasTransacoes>();
        services.AddScoped<ObterSaldoTotal>();
        services.AddScoped<ExcluirTransacao>(); // NOVO: Registro do Use Case ExcluirTransacao
        services.AddScoped<AtualizarTransacao>();


        // **CRÍTICO:** Registro da implementação do repositório.
        // ITransacaoRepository (interface) está em ContaComigo.Application.Interfaces
        // InMemoryTransacaoRepository (implementação) está em ContaComigo.Infrastructure.Repositories
        services.AddSingleton<ITransacaoRepository, InMemoryTransacaoRepository>();

        // Adiciona o HttpClient para a API (se o servidor for fazer chamadas para outras APIs)
        services.AddHttpClient();

        // Adiciona Swagger/OpenAPI para documentação da API
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    // Este método configura o pipeline de solicitação HTTP
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseWebAssemblyDebugging(); // Habilita a depuração do cliente Blazor
            app.UseSwagger(); // Para usar a interface do Swagger
            app.UseSwaggerUI(); // Para a UI do Swagger
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseBlazorFrameworkFiles(); // Serve os arquivos do framework Blazor WebAssembly
        app.UseStaticFiles(); // Serve arquivos estáticos do wwwroot

        app.UseRouting(); // Habilita o roteamento

        app.UseEndpoints(endpoints =>
        {
            // Mapeia os endpoints dos controladores (suas APIs)
            endpoints.MapControllers();
            // Para o fallback do Blazor e se tiver Razor Pages
            endpoints.MapRazorPages();
            // CRÍTICO: Para rotas não encontradas no servidor, redireciona para o index.html do cliente Blazor.
            endpoints.MapFallbackToFile("index.html");
        });
    }
}