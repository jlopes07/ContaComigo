// ContaComigo.Tests/ObterTodasTransacoesTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using ContaComigo.Application.Interfaces;
using ContaComigo.Application.UseCases.Transacoes; // Onde o novo caso de uso estará
using ContaComigo.Shared.Models;
using System.Collections.Generic;
using System.Linq;

public class ObterTodasTransacoesTests
{
    [Fact]
    public void Executar_DeveRetornarTodasAsTransacoesDoRepositorio()
    {
        // Arrange
        // Criar um mock para ITransacaoRepository
        var mockRepository = new Mock<ITransacaoRepository>();

        // Preparar algumas transações de exemplo que o repositório "simulará" que tem
        var transacoesEsperadas = new List<Transacao>
        {
            new Transacao("Aluguel", 1500.00m, new DateTime(2023, 1, 1)),
            new Transacao("Salário", 3000.00m, new DateTime(2023, 1, 5)),
            new Transacao("Transporte", 50.00m, new DateTime(2023, 1, 10))
        };

        // Configurar o mock para retornar as transações esperadas quando o método ObterTodas for chamado
        mockRepository.Setup(repo => repo.ObterTodas()).Returns(transacoesEsperadas);

        // Criar uma instância do novo caso de uso
        var obterTodasTransacoes = new ObterTodasTransacoes(mockRepository.Object); // Este "ObterTodasTransacoes" ainda não existe!

        // Act
        // Executar o método que estamos testando
        IEnumerable<Transacao> resultado = obterTodasTransacoes.Executar();

        // Assert
        // Verificar se o método ObterTodas do repositório foi chamado exatamente uma vez
        mockRepository.Verify(repo => repo.ObterTodas(), Times.Once);

        // Verificar se o resultado contém as transações esperadas
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(transacoesEsperadas.Count);
        resultado.Should().BeEquivalentTo(transacoesEsperadas); // Compara se os objetos são equivalentes
    }
}