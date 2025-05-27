// ContaComigo.Application/UseCases/Transacoes/AtualizarTransacao.cs

using ContaComigo.Application.Interfaces;
using ContaComigo.Shared.Models;
using System;

namespace ContaComigo.Application.UseCases.Transacoes;

public class AtualizarTransacao
{
    private readonly ITransacaoRepository _transacaoRepository;

    public AtualizarTransacao(ITransacaoRepository transacaoRepository)
    {
        _transacaoRepository = transacaoRepository;
    }

    public bool Executar(Transacao transacaoAtualizada)
    {
        // Validações de negócio adicionais podem ir aqui antes de chamar o repositório.
        // Por exemplo, verificar se a transaçãoAtualizada.Id não é Guid.Empty.

        // Se a transação for de saída, garantir que o valor seja negativo
        if (transacaoAtualizada.Tipo == TipoTransacao.Saida && transacaoAtualizada.Valor > 0)
        {
            transacaoAtualizada.Valor = -transacaoAtualizada.Valor;
        }

        // Chame o método Update no repositório. A lógica de "não encontrada"
        // será tratada pela implementação do repositório, que o Use Case irá verificar.
        var transacaoExistente = _transacaoRepository.GetById(transacaoAtualizada.Id);
        if (transacaoExistente == null)
        {
            Console.WriteLine($"DEBUG (AtualizarTransacao UseCase): Transação com ID {transacaoAtualizada.Id} não encontrada para atualização.");
            return false; // Indica que a transação não existia
        }

        _transacaoRepository.Update(transacaoAtualizada);
        Console.WriteLine($"DEBUG (AtualizarTransacao UseCase): Transação com ID {transacaoAtualizada.Id} atualizada com sucesso.");
        return true; // Indica que a transação foi atualizada
    }
}