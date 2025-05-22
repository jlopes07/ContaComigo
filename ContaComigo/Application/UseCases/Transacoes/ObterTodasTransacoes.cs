using ContaComigo.Application.Interfaces;
using ContaComigo.Shared.Models;
using System.Collections.Generic;

namespace ContaComigo.Application.UseCases.Transacoes
{
    public class ObterTodasTransacoes
    {
        private readonly ITransacaoRepository _transacaoRepository;

        public ObterTodasTransacoes(ITransacaoRepository transacaoRepository)
        {
            _transacaoRepository = transacaoRepository;
        }

        public IEnumerable<Transacao> Executar()
        {
            // O nome do método no repositório foi alterado para GetAll()
            return _transacaoRepository.GetAll();
        }
    }
}