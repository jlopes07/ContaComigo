// ContaComigo.Tests/TransacoesControllerIntegrationTests.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json; // Adicione este using para JsonSerializer
using System.Threading.Tasks; // Mantenha este using, pois HttpClient.GetAsync � ass�ncrono
using ContaComigo.Shared.Models;
using FluentAssertions;
using FluentAssertions.Web;
using Microsoft.AspNetCore.Mvc.Testing; // Para CustomWebApplicationFactory
using Xunit;
using Moq;
using ContaComigo; // **CRUCIAL**: Adicione este using para encontrar 'Program' e 'CustomWebApplicationFactory'
using Microsoft.Extensions.DependencyInjection;

namespace ContaComigo.Tests
{
    public class TransacoesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;
        // O mock deve ser inicializado, por isso n�o � 'readonly' direto na declara��o,
        // mas sim no construtor.
        private readonly Mock<ContaComigo.Application.Interfaces.ITransacaoRepository> TransacaoRepositoryMock;

        public TransacoesControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            TransacaoRepositoryMock = new Mock<ContaComigo.Application.Interfaces.ITransacaoRepository>();

            // Configura o factory para usar o mock em vez da implementa��o real do reposit�rio
            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Encontre e remova a declara��o existente do ITransacaoRepository
                    // (que seria o InMemoryTransacaoRepository em sua aplica��o real)
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(ContaComigo.Application.Interfaces.ITransacaoRepository));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Adicione o nosso mock do reposit�rio.
                    // Agora, quando o Controller pedir um ITransacaoRepository, ele receber� nosso mock.
                    services.AddSingleton(TransacaoRepositoryMock.Object);
                });
            }).CreateClient();
        }

        // Teste de integra��o para GET /api/transacoes
        [Fact]
        public async Task Get_Transacoes_DeveRetornarListaDeTransacoes()
        {
            // Arrange
            // Estas s�o as transa��es que o teste *espera* que a API retorne.
            // Os IDs s�o gerados automaticamente pelo construtor de Transacao.
            var expectedTransacoes = new List<Transacao>
            {
                new Transacao("Aluguel", 1500m, new DateTime(2023, 1, 1)),
                new Transacao("Sal�rio", 3000m, new DateTime(2023, 1, 5)),
                new Transacao("Compras", 250m, new DateTime(2023, 1, 10))
            };

            // Configura o mock do reposit�rio.
            // Quando a API (o TransacoesController) chamar ObterTodas() no ITransacaoRepository,
            // o mock vai interceptar essa chamada e retornar a 'expectedTransacoes'.
            // Usamos .Returns() porque ObterTodas() � um m�todo s�ncrono.
            TransacaoRepositoryMock.Setup(repo => repo.ObterTodas())
                                   .Returns(expectedTransacoes);

            // Act
            // Faz a requisi��o HTTP real para a API rodando em mem�ria.
            var response = await _client.GetAsync("/api/transacoes");

            // Assert
            // Primeiro, verifica se a resposta HTTP foi 200 OK.
            response.Should().Be200Ok();

            // Deserializa o conte�do da resposta de string JSON para uma lista de Transacao.
            // Usamos JsonSerializer.Deserialize com PropertyNameCaseInsensitive para maior robustez.
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var transacoesRetornadas = JsonSerializer.Deserialize<List<Transacao>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Usa FluentAssertions para comparar as listas.
            // O .Excluding(t => t.Id) � crucial aqui: ele diz para ignorar a compara��o do campo Id,
            // pois os IDs s�o gerados dinamicamente no construtor da Transacao, e o foco � testar
            // se os outros dados de neg�cio foram retornados corretamente.
            transacoesRetornadas.Should().BeEquivalentTo(expectedTransacoes, options => options
                .ComparingByMembers<Transacao>()
                .Excluding(t => t.Id));

            // Verifica se o m�todo ObterTodas() foi chamado no mock exatamente uma vez.
            // Isso confirma que o TransacoesController realmente tentou buscar os dados do reposit�rio.
            TransacaoRepositoryMock.Verify(repo => repo.ObterTodas(), Times.Once);
        }

        // Voc� pode adicionar mais testes de integra��o aqui conforme o projeto avan�a.
        // Exemplo: Teste de integra��o para POST /api/transacoes
        // [Fact]
        // public async Task Post_Transacao_DeveRegistrar()
        // {
        //     // Arrange
        //     var novaTransacao = new Transacao("Teste POST", 100m, DateTime.Now);
        //
        //     // Configure o mock para o m�todo Adicionar (que � void)
        //     TransacaoRepositoryMock.Setup(repo => repo.Adicionar(It.IsAny<Transacao>()));
        //
        //     // Act
        //     var response = await _client.PostAsJsonAsync("/api/transacoes", novaTransacao);
        //
        //     // Assert
        //     // Exemplo: espera 201 Created se a cria��o for bem-sucedida
        //     response.Should().Be201Created();
        //     // Verifica se o m�todo Adicionar foi chamado no mock
        //     TransacaoRepositoryMock.Verify(repo => repo.Adicionar(It.IsAny<Transacao>()), Times.Once);
        // }
    }
}