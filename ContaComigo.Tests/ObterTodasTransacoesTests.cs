using Xunit;
using Moq;
using ContaComigo.Application.Interfaces;
using ContaComigo.Application.UseCases.Transacoes;
using ContaComigo.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ContaComigo.Tests
{
    public class ObterTodasTransacoesTests
    {
        [Fact]
        public void Executar_DeveRetornarTodasAsTransacoes()
        {
            // Arrange
            var mockTransacaoRepository = new Mock<ITransacaoRepository>();
            // Adicione Tipo e Categoria aos construtores
            var transacoesEsperadas = new List<Transacao>
            {
                new Transacao("Conta de Luz", 150.00m, new DateTime(2025, 3, 10), TipoTransacao.Saida, CategoriaTransacao.Outros),
                new Transacao("Internet", 80.00m, new DateTime(2025, 3, 15), TipoTransacao.Saida, CategoriaTransacao.Outros),
                new Transacao("Presente", 200.00m, new DateTime(2025, 3, 20), TipoTransacao.Saida, CategoriaTransacao.Lazer)
            };

            // Alterado de ObterTodas para GetAll
            mockTransacaoRepository.Setup(r => r.GetAll()).Returns(transacoesEsperadas);

            var obterTodasTransacoes = new ObterTodasTransacoes(mockTransacaoRepository.Object);

            // Act
            var transacoesReais = obterTodasTransacoes.Executar();

            // Assert
            Assert.NotNull(transacoesReais);
            Assert.Equal(transacoesEsperadas.Count, transacoesReais.Count());
            Assert.Contains(transacoesEsperadas[0], transacoesReais);
        }

        [Fact]
        public void Executar_DeveRetornarListaVazia_QuandoNaoHaTransacoes()
        {
            // Arrange
            var mockTransacaoRepository = new Mock<ITransacaoRepository>();
            // Alterado de ObterTodas para GetAll
            mockTransacaoRepository.Setup(r => r.GetAll()).Returns(new List<Transacao>());

            var obterTodasTransacoes = new ObterTodasTransacoes(mockTransacaoRepository.Object);

            // Act
            var transacoesReais = obterTodasTransacoes.Executar();

            // Assert
            Assert.NotNull(transacoesReais);
            Assert.Empty(transacoesReais);
        }
    }
}