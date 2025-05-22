using Xunit;
using Moq;
using ContaComigo.Application.Interfaces;
using ContaComigo.Application.UseCases.Transacoes;
using ContaComigo.Shared.Models;
using System;

namespace ContaComigo.Tests
{
    public class RegistrarTransacaoTests
    {
        [Fact]
        public void Executar_DeveAdicionarTransacao_QuandoTransacaoValida()
        {
            // Arrange
            var mockTransacaoRepository = new Mock<ITransacaoRepository>();
            var registrarTransacao = new RegistrarTransacao(mockTransacaoRepository.Object);

            // Adicione Tipo e Categoria ao construtor
            var transacao = new Transacao("Salário", 1000m, DateTime.Now, TipoTransacao.Entrada, CategoriaTransacao.Salario);

            // Act
            registrarTransacao.Executar(transacao);

            // Assert
            // Alterado de Adicionar para Add
            mockTransacaoRepository.Verify(r => r.Add(transacao), Times.Once);
        }

        [Fact]
        public void Executar_DeveLancarArgumentNullException_QuandoTransacaoNula()
        {
            // Arrange
            var mockTransacaoRepository = new Mock<ITransacaoRepository>();
            var registrarTransacao = new RegistrarTransacao(mockTransacaoRepository.Object);

            Transacao transacao = null; // Testando com transação nula

            // Act & Assert
            // Verifica se a exceção ArgumentNullException é lançada quando a transação é nula
            Assert.Throws<ArgumentNullException>(() => registrarTransacao.Executar(transacao));

            // Garante que o método Add não foi chamado no repositório
            // Alterado de Adicionar para Add
            mockTransacaoRepository.Verify(r => r.Add(It.IsAny<Transacao>()), Times.Never);
        }

        [Fact]
        public void Executar_DeveNegativarValor_QuandoTipoForSaida()
        {
            // Arrange
            var mockTransacaoRepository = new Mock<ITransacaoRepository>();
            var registrarTransacao = new RegistrarTransacao(mockTransacaoRepository.Object);

            // Crie uma transação de saída com valor positivo
            // Adicione Tipo e Categoria ao construtor
            var transacaoSaida = new Transacao("Aluguel", 1500m, DateTime.Now, TipoTransacao.Saida, CategoriaTransacao.Aluguel);

            // Act
            registrarTransacao.Executar(transacaoSaida);

            // Assert
            // Verifica se o valor da transação foi negativado antes de ser adicionado
            // Alterado de Adicionar para Add
            mockTransacaoRepository.Verify(r => r.Add(It.Is<Transacao>(t => t.Valor == -1500m)), Times.Once);
        }

        [Fact]
        public void Executar_NaoDeveAlterarValor_QuandoTipoForEntrada()
        {
            // Arrange
            var mockTransacaoRepository = new Mock<ITransacaoRepository>();
            var registrarTransacao = new RegistrarTransacao(mockTransacaoRepository.Object);

            // Crie uma transação de entrada com valor positivo
            // Adicione Tipo e Categoria ao construtor
            var transacaoEntrada = new Transacao("Freelance", 500m, DateTime.Now, TipoTransacao.Entrada, CategoriaTransacao.Salario);

            // Act
            registrarTransacao.Executar(transacaoEntrada);

            // Assert
            // Verifica se o valor da transação permaneceu positivo (não foi alterado)
            // Alterado de Adicionar para Add
            mockTransacaoRepository.Verify(r => r.Add(It.Is<Transacao>(t => t.Valor == 500m)), Times.Once);
        }

        // Teste adicional para garantir que valores negativos não são duplicadamente negativados
        [Fact]
        public void Executar_NaoDeveAlterarValor_SeJaNegativoESaida()
        {
            // Arrange
            var mockTransacaoRepository = new Mock<ITransacaoRepository>();
            var registrarTransacao = new RegistrarTransacao(mockTransacaoRepository.Object);

            // Cria uma transação de saída com um valor já negativo (cenário menos comum, mas para robustez)
            var transacaoSaidaPreNegativada = new Transacao("Compra", -50m, DateTime.Now, TipoTransacao.Saida, CategoriaTransacao.Alimentacao);

            // Act
            registrarTransacao.Executar(transacaoSaidaPreNegativada);

            // Assert
            // O valor deve permanecer -50m (não deve virar 50m ou -(-50m))
            mockTransacaoRepository.Verify(r => r.Add(It.Is<Transacao>(t => t.Valor == -50m)), Times.Once);
        }
    }
}