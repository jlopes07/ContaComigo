// ContaComigo.Tests/RegistrarTransacaoTests.cs

using Xunit;
using Moq; // Para usar Mock
using FluentAssertions; // Para usar os métodos de asserção do FluentAssertions
using System;
using ContaComigo.Application.Interfaces; // Para ITransacaoRepository
using ContaComigo.Application.UseCases.Transacoes; // Para RegistrarTransacao
using ContaComigo.Shared.Models; // Para Transacao

namespace ContaComigo.Tests
{
    public class RegistrarTransacaoTests
    {
        private readonly Mock<ITransacaoRepository> _mockTransacaoRepository;
        private readonly RegistrarTransacao _registrarTransacao;

        public RegistrarTransacaoTests()
        {
            _mockTransacaoRepository = new Mock<ITransacaoRepository>();
            _registrarTransacao = new RegistrarTransacao(_mockTransacaoRepository.Object);
        }

        [Fact]
        public void Executar_ComTransacaoValida_DeveChamarAdicionarNoRepositorio()
        {
            // Arrange
            var transacao = new Transacao("Salário", 3000m, DateTime.Now);

            // Act
            _registrarTransacao.Executar(transacao);

            // Assert
            _mockTransacaoRepository.Verify(repo => repo.Adicionar(transacao), Times.Once);
        }

        [Fact]
        public void Executar_ComDescricaoVazia_DeveLancarArgumentException()
        {
            // Arrange (não precisamos de um mock aqui, o problema é na criação da Transacao)
            // A Action serve para encapsular o código que pode lançar a exceção
            Action act = () => new Transacao("", 100, DateTime.Now);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("A descrição da transação não pode ser nula ou vazia. (Parameter 'descricao')");

            // Verifica que o método Adicionar do repositório NÃO foi chamado
            _mockTransacaoRepository.Verify(repo => repo.Adicionar(It.IsAny<Transacao>()), Times.Never);
        }

        [Fact]
        public void Executar_ComDescricaoNula_DeveLancarArgumentException()
        {
            // Arrange
            Action act = () => new Transacao(null, 100, DateTime.Now);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("A descrição da transação não pode ser nula ou vazia. (Parameter 'descricao')");

            _mockTransacaoRepository.Verify(repo => repo.Adicionar(It.IsAny<Transacao>()), Times.Never);
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(0)]
        public void Executar_ComValorInvalido_DeveLancarArgumentException(decimal valorInvalido)
        {
            // Arrange
            Action act = () => new Transacao("Teste Valor Inválido", valorInvalido, DateTime.Now);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("O valor da transação deve ser maior que zero. (Parameter 'valor')");

            _mockTransacaoRepository.Verify(repo => repo.Adicionar(It.IsAny<Transacao>()), Times.Never);
        }

        [Fact]
        public void Construtor_NaoDeveAceitarRepositorioNulo()
        {
            Action act = () => new RegistrarTransacao(null);

            act.Should().Throw<ArgumentNullException>()
               // Mude a mensagem para a que você usou no construtor de RegistrarTransacao
               .WithMessage("O repositório de transações não pode ser nulo. (Parameter 'transacaoRepository')");
        }

        // Você pode ter outros testes aqui para cenários válidos, como:
        [Fact]
        public void Executar_TransacaoComTodosCamposValidos_DeveSerRegistradaCorretamente()
        {
            // Arrange
            var transacao = new Transacao("Aluguel", 1500.00m, new DateTime(2023, 1, 15));

            // Act
            _registrarTransacao.Executar(transacao);

            // Assert (verificamos se o método Adicionar do mock foi chamado com a transação correta)
            _mockTransacaoRepository.Verify(repo => repo.Adicionar(It.Is<Transacao>(t =>
                t.Descricao == "Aluguel" &&
                t.Valor == 1500.00m &&
                t.Data == new DateTime(2023, 1, 15))), Times.Once);
        }
    }
}