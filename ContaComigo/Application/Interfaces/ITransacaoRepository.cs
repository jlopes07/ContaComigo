using ContaComigo.Shared.Models;
using System;
using System.Collections.Generic;

namespace ContaComigo.Application.Interfaces;

public interface ITransacaoRepository
{
    IEnumerable<Transacao> GetAll(); // Mudado de 'ObterTodas' para 'GetAll'
    void Add(Transacao transacao);   // Mudado de 'Adicionar' para 'Add'
    Transacao? GetById(Guid id);     // Mudado para retornar Transacao? (anulável)
}