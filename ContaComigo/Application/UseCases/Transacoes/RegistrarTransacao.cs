// ContaComigo.Application/UseCases/Transacoes/RegistrarTransacao.cs
using ContaComigo.Application.Interfaces;
using ContaComigo.Shared.Models;
using System;

namespace ContaComigo.Application.UseCases.Transacoes
{
    public class RegistrarTransacao
    {
        private readonly ITransacaoRepository _transacaoRepository;

        public RegistrarTransacao(ITransacaoRepository transacaoRepository)
        {
            _transacaoRepository = transacaoRepository;
        }

        public void Executar(Transacao transacao)
        {
            if (transacao == null)
            {
                throw new ArgumentNullException(nameof(transacao));
            }

            // Lógica para ajustar o valor com base no tipo de transação
            if (transacao.Tipo == TipoTransacao.Saida && transacao.Valor > 0)
            {
                transacao.Valor = -transacao.Valor; // Torna o valor negativo para saídas
            }
            // Se for Entrada, o valor já é positivo e permanece assim.

            _transacaoRepository.Add(transacao);
        }
    }
}