// ContaComigo/Application/UseCases/Transacoes/RegistrarTransacao.cs

using ContaComigo.Application.Interfaces; // Para ITransacaoRepository
using ContaComigo.Shared.Models; // Para Transacao
using System; // Para ArgumentNullException

namespace ContaComigo.Application.UseCases.Transacoes
{
    public class RegistrarTransacao
    {
        private readonly ITransacaoRepository _transacaoRepository;

        public RegistrarTransacao(ITransacaoRepository transacaoRepository)
        {
            // Adicione esta validação:
            _transacaoRepository = transacaoRepository ?? throw new ArgumentNullException(nameof(transacaoRepository), "O repositório de transações não pode ser nulo.");
        }

        public void Executar(Transacao transacao)
        {
            // A validação de 'transacao' em si já está no construtor da Transacao
            // então não precisamos duplicar aqui, a menos que haja uma regra de negócio adicional.

            _transacaoRepository.Adicionar(transacao);
        }
    }
}