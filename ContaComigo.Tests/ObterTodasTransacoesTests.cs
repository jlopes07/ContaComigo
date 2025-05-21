// ContaComigo.Tests/ObterTodasTransacoesTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using ContaComigo.Application.Interfaces;
using ContaComigo.Application.UseCases.Transacoes; // Onde o novo caso de uso estar�
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

        // Preparar algumas transa��es de exemplo que o reposit�rio "simular�" que tem
        var transacoesEsperadas = new List<Transacao>
        {
            new Transacao("Aluguel", 1500.00m, new DateTime(2023, 1, 1)),
            new Transacao("Sal�rio", 3000.00m, new DateTime(2023, 1, 5)),
            new Transacao("Transporte", 50.00m, new DateTime(2023, 1, 10))
        };

        // Configurar o mock para retornar as transa��es esperadas quando o m�todo ObterTodas for chamado
        mockRepository.Setup(repo => repo.ObterTodas()).Returns(transacoesEsperadas);

        // Criar uma inst�ncia do novo caso de uso
        var obterTodasTransacoes = new ObterTodasTransacoes(mockRepository.Object); // Este "ObterTodasTransacoes" ainda n�o existe!

        // Act
        // Executar o m�todo que estamos testando
        IEnumerable<Transacao> resultado = obterTodasTransacoes.Executar();

        // Assert
        // Verificar se o m�todo ObterTodas do reposit�rio foi chamado exatamente uma vez
        mockRepository.Verify(repo => repo.ObterTodas(), Times.Once);

        // Verificar se o resultado cont�m as transa��es esperadas
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(transacoesEsperadas.Count);
        resultado.Should().BeEquivalentTo(transacoesEsperadas); // Compara se os objetos s�o equivalentes
    }
}