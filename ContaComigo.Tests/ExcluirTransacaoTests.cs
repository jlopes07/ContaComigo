// ContaComigo.Tests/ExcluirTransacaoTests.cs

using Xunit;
using Moq; // Para criar mocks do repositório
using ContaComigo.Application.Interfaces;
using ContaComigo.Application.UseCases.Transacoes;
using ContaComigo.Shared.Models; // Se precisar de Transacao, mas para ExcluirTransacao UseCase não é diretamente usado.
using System;

namespace ContaComigo.Tests;

public class ExcluirTransacaoTests
{
    [Fact]
    public void Executar_DeveChamarDeleteNoRepositorio_QuandoTransacaoExiste()
    {
        // Arrange
        var mockTransacaoRepository = new Mock<ITransacaoRepository>();
        var transacaoId = Guid.NewGuid();

        // Configura o mock para retornar true (indicando que a exclusão foi bem-sucedida)
        mockTransacaoRepository.Setup(r => r.Delete(transacaoId)).Returns(true);

        var excluirTransacao = new ExcluirTransacao(mockTransacaoRepository.Object);

        // Act
        var result = excluirTransacao.Executar(transacaoId);

        // Assert
        // Verifica se o método Delete foi chamado no repositório com o ID correto
        mockTransacaoRepository.Verify(r => r.Delete(transacaoId), Times.Once);
        // Verifica se o resultado da execução é verdadeiro
        Assert.True(result);
    }

    [Fact]
    public void Executar_DeveRetornarFalso_QuandoTransacaoNaoExiste()
    {
        // Arrange
        var mockTransacaoRepository = new Mock<ITransacaoRepository>();
        var transacaoId = Guid.NewGuid();

        // Configura o mock para retornar false (indicando que a transação não foi encontrada)
        mockTransacaoRepository.Setup(r => r.Delete(transacaoId)).Returns(false);

        var excluirTransacao = new ExcluirTransacao(mockTransacaoRepository.Object);

        // Act
        var result = excluirTransacao.Executar(transacaoId);

        // Assert
        // Verifica se o método Delete foi chamado no repositório com o ID correto
        mockTransacaoRepository.Verify(r => r.Delete(transacaoId), Times.Once);
        // Verifica se o resultado da execução é falso
        Assert.False(result);
    }
}