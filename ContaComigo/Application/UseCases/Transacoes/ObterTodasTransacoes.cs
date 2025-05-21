using ContaComigo.Application.Interfaces;
using ContaComigo.Shared.Models;
using System.Collections.Generic;

namespace ContaComigo.Application.UseCases.Transacoes;

public class ObterTodasTransacoes
{
    private readonly ITransacaoRepository _repository;

    public ObterTodasTransacoes(ITransacaoRepository repository)
    {
        _repository = repository;
    }

    public IEnumerable<Transacao> Executar()
    {
        // Delega a responsabilidade de obter as transações ao repositório
        return _repository.ObterTodas();
    }
}