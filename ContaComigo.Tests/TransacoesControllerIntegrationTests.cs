using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using ContaComigo.Shared.Models;
using System.Net.Http.Json;
using System.Collections.Generic;
using ContaComigo.Infrastructure.Repositories; // Para usar InMemoryTransacaoRepository
using ContaComigo.Application.Interfaces; // Para ITransacaoRepository
using Microsoft.Extensions.DependencyInjection; // Para criar escopo de serviço
using System.Linq;
using System;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ContaComigo.Tests
{
    // WebApplicationFactory é um utilitário para testar APIs ASP.NET Core
    public class TransacoesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public TransacoesControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Substitua a implementação do repositório em memória para que os testes não interfiram nos dados estáticos
                    // Criaremos uma nova instância para cada teste
                    services.AddSingleton<ITransacaoRepository, InMemoryTransacaoRepository>();
                });
            });
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Get_DeveRetornarTransacoesExistentes()
        {
            // Arrange
            // Os dados iniciais estão no InMemoryTransacaoRepository
            // Adicione Tipo e Categoria
            var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
            repo.Add(new Transacao("Teste Get 1", 100m, DateTime.Now, TipoTransacao.Entrada, CategoriaTransacao.Outros));
            repo.Add(new Transacao("Teste Get 2", -50m, DateTime.Now, TipoTransacao.Saida, CategoriaTransacao.Alimentacao));

            // Act
            var response = await _client.GetAsync("/api/transacoes");

            // Assert
            response.EnsureSuccessStatusCode();
            var transacoes = await response.Content.ReadFromJsonAsync<List<Transacao>>();
            Assert.NotNull(transacoes);
            // Verifica se as transações iniciais foram carregadas
            // Alterado de ObterTodas para GetAll
            Assert.True(transacoes.Count() >= 2); // Pode ter dados pre-existentes + os adicionados
            Assert.Contains(transacoes, t => t.Descricao == "Teste Get 1");
            Assert.Contains(transacoes, t => t.Descricao == "Teste Get 2");
        }

        [Fact]
        public async Task Post_DeveCriarNovaTransacao_E_RetornarCreated()
        {
            // Arrange
            // Adicione Tipo e Categoria ao DTO
            var novaTransacaoDto = new TransacaoDto
            {
                Descricao = "Compra Teste",
                Valor = 200.00m,
                Data = DateTime.Now,
                Tipo = TipoTransacao.Saida,
                Categoria = CategoriaTransacao.Lazer
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/transacoes", novaTransacaoDto);

            // Assert
            response.EnsureSuccessStatusCode(); // Status 2xx
            Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

            var transacaoCriada = await response.Content.ReadFromJsonAsync<Transacao>();
            Assert.NotNull(transacaoCriada);
            Assert.NotEqual(Guid.Empty, transacaoCriada.Id);
            Assert.Equal(novaTransacaoDto.Descricao, transacaoCriada.Descricao);
            // Verifica se o valor foi negativado corretamente no servidor
            Assert.Equal(-novaTransacaoDto.Valor, transacaoCriada.Valor);
            Assert.Equal(novaTransacaoDto.Tipo, transacaoCriada.Tipo);
            Assert.Equal(novaTransacaoDto.Categoria, transacaoCriada.Categoria);

            // Opcional: Verificar se a transação está no repositório após a criação
            var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
            // Alterado de ObterTodas para GetAll
            Assert.Contains(repo.GetAll(), t => t.Id == transacaoCriada.Id);
        }

        [Fact]
        public async Task GetSaldo_DeveRetornarSaldoCorreto()
        {
            // Arrange
            var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
            // Limpa o repositório para ter controle total sobre os dados de teste para o saldo
            (repo as InMemoryTransacaoRepository)?.Clear(); // Assumindo que você adicione um método Clear no InMemoryTransacaoRepository

            // Adicione transações para o cálculo do saldo
            repo.Add(new Transacao("Depósito", 1000m, DateTime.Now, TipoTransacao.Entrada, CategoriaTransacao.Salario));
            repo.Add(new Transacao("Conta de Água", 120m, DateTime.Now, TipoTransacao.Saida, CategoriaTransacao.Outros));
            repo.Add(new Transacao("Reembolso", 50m, DateTime.Now, TipoTransacao.Entrada, CategoriaTransacao.Outros));
            repo.Add(new Transacao("Lanche", 30m, DateTime.Now, TipoTransacao.Saida, CategoriaTransacao.Alimentacao));

            // O RegistrarTransacao é que negativou, então os valores já devem estar assim no repositório para o teste.
            // Se você chamar .Add diretamente, o InMemoryTransacaoRepository vai apenas adicionar.
            // A soma esperada deve considerar os valores como eles são armazenados no repositório.
            // Para garantir que o teste seja robusto, considere usar o UseCase RegistrarTransacao para adicionar as transações
            // ou garantir que os valores já estejam com o sinal correto ao adicionar.
            // Para este teste, vamos assumir que o "Add" do repositório já recebe o valor com o sinal correto.
            decimal expectedSaldo = 1000m - 120m + 50m - 30m; // 900m

            // Act
            var response = await _client.GetAsync("/api/transacoes/saldo");

            // Assert
            response.EnsureSuccessStatusCode();
            var saldo = await response.Content.ReadFromJsonAsync<decimal>();
            Assert.Equal(expectedSaldo, saldo);
        }
    }

    // Extensão para InMemoryTransacaoRepository para limpar os dados em testes
    // Você pode adicionar este método no arquivo ContaComigo.Infrastructure/Repositories/InMemoryTransacaoRepository.cs
    // Se você já não o tiver, adicione-o lá.
    // Isso é útil para garantir que cada teste de integração rode com um estado limpo.
    public static class InMemoryTransacaoRepositoryExtensions
    {
        public static void Clear(this InMemoryTransacaoRepository repo)
        {
            var field = typeof(InMemoryTransacaoRepository)
                .GetField("_transacoes", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                var list = field.GetValue(null) as System.Collections.IList;
                list?.Clear();
            }
        }
    }
}