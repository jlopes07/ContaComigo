// ContaComigo.Application/UseCases/Transacoes/ExcluirTransacao.cs

using ContaComigo.Application.Interfaces;
using System;

namespace ContaComigo.Application.UseCases.Transacoes;

public class ExcluirTransacao
{
    private readonly ITransacaoRepository _transacaoRepository;

    public ExcluirTransacao(ITransacaoRepository transacaoRepository)
    {
        _transacaoRepository = transacaoRepository;
    }

    public bool Executar(Guid id)
    {
        // A lógica de negócio para exclusão, se houver, iria aqui.
        // Por exemplo, verificar permissões, regras de negócio antes de deletar.

        // Chama o método Delete no repositório
        var isDeleted = _transacaoRepository.Delete(id);

        if (!isDeleted)
        {
            Console.WriteLine($"DEBUG (ExcluirTransacao UseCase): Transação com ID {id} não encontrada para exclusão.");
        }
        else
        {
            Console.WriteLine($"DEBUG (ExcluirTransacao UseCase): Transação com ID {id} excluída com sucesso.");
        }

        return isDeleted;
    }
}