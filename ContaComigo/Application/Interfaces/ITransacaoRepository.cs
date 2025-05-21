using ContaComigo.Shared.Models;
using System; // Certifique-se de ter este using para Guid
using System.Collections.Generic; // Certifique-se de ter este using para IEnumerable
using System.Threading.Tasks;

namespace ContaComigo.Application.Interfaces;

public interface ITransacaoRepository
{
    void Adicionar(Transacao transacao);
    IEnumerable<Transacao> ObterTodas();
    Transacao? ObterPorId(Guid id); // Alterado para retornar Transacao? para compatibilidade com a implementação
    // void Atualizar(Transacao transacao); // Métodos futuros
    // void Remover(Guid id); // Métodos futuros
}