using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using ContaComigo.Application.UseCases.Transacoes;
using ContaComigo.Application.Interfaces;
using ContaComigo.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Adiciona serviços ao contêiner.
builder.Services.AddControllersWithViews(); // Necessário para seus controladores de API
builder.Services.AddRazorPages(); // Necessário para o fallback do Blazor e se tiver Razor Pages

// **Registro dos seus Use Cases e Repositórios**
// Estes são os serviços que seus controladores usarão.
builder.Services.AddScoped<RegistrarTransacao>();
builder.Services.AddScoped<ObterTodasTransacoes>();
builder.Services.AddScoped<ObterSaldoTotal>(); // Registro do novo Use Case para o saldo

// **CRÍTICO:** Registro da implementação do repositório.
// ITransacaoRepository (interface) está em ContaComigo.Application.Interfaces
// InMemoryTransacaoRepository (implementação) está em ContaComigo.Infrastructure.Repositories
builder.Services.AddSingleton<ITransacaoRepository, InMemoryTransacaoRepository>();

// Adiciona o HttpClient para a API (se o servidor for fazer chamadas para outras APIs)
builder.Services.AddHttpClient();


// Adiciona Swagger/OpenAPI para documentação da API (opcional, se você quiser testar os endpoints da API separadamente)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o pipeline de solicitação HTTP.
if (app.Environment.IsDevelopment())
{
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

// Mapeia os endpoints dos controladores (suas APIs)
app.MapControllers();
app.MapRazorPages(); // Para o fallback do Blazor e se tiver Razor Pages

// CRÍTICO: Para rotas não encontradas no servidor, redireciona para o index.html do cliente Blazor.
app.MapFallbackToFile("index.html");

app.Run();