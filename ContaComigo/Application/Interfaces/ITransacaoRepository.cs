// ContaComigo.Application/Interfaces/ITransacaoRepository.cs

using ContaComigo.Shared.Models;
using System;
using System.Collections.Generic;

namespace ContaComigo.Application.Interfaces;

public interface ITransacaoRepository
{
    // Métodos existentes (GetAll, GetById, Add, Update)
    IEnumerable<Transacao> GetAll();
    Transacao GetById(Guid id);
    void Add(Transacao transacao);
    void Update(Transacao transacao);

    // NOVO: Método para excluir uma transação
    bool Delete(Guid id); // <--- Adicione esta linha
}