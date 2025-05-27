// ContaComigo.Tests/AtualizarTransacaoTests.cs

using Xunit;
using Moq;
using ContaComigo.Application.Interfaces;
using ContaComigo.Application.UseCases.Transacoes;
using ContaComigo.Shared.Models;
using System;

namespace ContaComigo.Tests;

public class AtualizarTransacaoTests
{
    [Fact]
    public void Executar_DeveChamarUpdateNoRepositorio_QuandoTransacaoExiste()
    {
        // Arrange
        var mockTransacaoRepository = new Mock<ITransacaoRepository>();
        var transacaoOriginal = new Transacao("Conta Antiga", 100m, DateTime.Now, TipoTransacao.Saida, CategoriaTransacao.Outros);
        var transacaoAtualizada = new Transacao("Conta Nova", 120m, DateTime.Now.AddDays(1), TipoTransacao.Saida, CategoriaTransacao.Outros)
        {
            Id = transacaoOriginal.Id // Garante que o ID é o mesmo para atualização
        };

        // Configura o mock para que GetById retorne a transação original
        mockTransacaoRepository.Setup(r => r.GetById(transacaoOriginal.Id)).Returns(transacaoOriginal);

        var atualizarTransacao = new AtualizarTransacao(mockTransacaoRepository.Object);

        // Act
        var result = atualizarTransacao.Executar(transacaoAtualizada);

        // Assert
        // Verifica se o método Update foi chamado no repositório exatamente uma vez com a transação atualizada
        mockTransacaoRepository.Verify(r => r.Update(It.Is<Transacao>(t =>
            t.Id == transacaoAtualizada.Id &&
            t.Descricao == transacaoAtualizada.Descricao &&
            t.Valor == transacaoAtualizada.Valor &&
            t.Tipo == transacaoAtualizada.Tipo &&
            t.Categoria == transacaoAtualizada.Categoria
        )), Times.Once);
        Assert.True(result); // Espera que a execução retorne true
    }

    [Fact]
    public void Executar_NaoDeveChamarUpdateNoRepositorio_QuandoTransacaoNaoExiste()
    {
        // Arrange
        var mockTransacaoRepository = new Mock<ITransacaoRepository>();
        var transacaoNaoExistente = new Transacao("Fantasma", 50m, DateTime.Now, TipoTransacao.Entrada, CategoriaTransacao.Outros);

        // Configura o mock para que GetById retorne null (indicando que a transação não foi encontrada)
        mockTransacaoRepository.Setup(r => r.GetById(transacaoNaoExistente.Id)).Returns((Transacao)null);

        var atualizarTransacao = new AtualizarTransacao(mockTransacaoRepository.Object);

        // Act
        var result = atualizarTransacao.Executar(transacaoNaoExistente);

        // Assert
        // Verifica que o método Update NUNCA foi chamado
        mockTransacaoRepository.Verify(r => r.Update(It.IsAny<Transacao>()), Times.Never);
        Assert.False(result); // Espera que a execução retorne false
    }

    [Fact]
    public void Executar_DeveNegativarValor_QuandoTipoForSaidaEValorPositivo()
    {
        // Arrange
        var mockTransacaoRepository = new Mock<ITransacaoRepository>();
        var transacaoSaidaPositiva = new Transacao("Compra", 100m, DateTime.Now, TipoTransacao.Saida, CategoriaTransacao.Alimentacao);
        transacaoSaidaPositiva.Id = Guid.NewGuid(); // Garante um ID para simular existente

        mockTransacaoRepository.Setup(r => r.GetById(transacaoSaidaPositiva.Id)).Returns(transacaoSaidaPositiva);

        var atualizarTransacao = new AtualizarTransacao(mockTransacaoRepository.Object);

        // Act
        atualizarTransacao.Executar(transacaoSaidaPositiva);

        // Assert
        // Verifica se o Update foi chamado com o valor negativado
        mockTransacaoRepository.Verify(r => r.Update(It.Is<Transacao>(t => t.Valor == -100m)), Times.Once);
    }
}