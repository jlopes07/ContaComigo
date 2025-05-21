// ContaComigo/Infrastructure/Repositories/InMemoryTransacaoRepository.cs

using ContaComigo.Application.Interfaces;
using ContaComigo.Shared.Models;
using System.Collections.Generic;
using System.Linq; // Essencial para FirstOrDefault
using System; // Essencial para Guid

namespace ContaComigo.Infrastructure.Repositories;

public class InMemoryTransacaoRepository : ITransacaoRepository
{
    // Usamos uma lista privada para simular o armazenamento em memória
    // Para testes de unidade isolados, cada instância do repositório pode ter sua própria lista.
    // Para um repositório em memória em cenário de aplicação real, talvez fosse estática ou Singleton.
    private readonly List<Transacao> _transacoes = new();

    public void Adicionar(Transacao transacao)
    {
        _transacoes.Add(transacao);
    }

    public IEnumerable<Transacao> ObterTodas()
    {
        // Retorna uma cópia somente leitura da lista para evitar modificações externas diretas
        return _transacoes.AsReadOnly();
    }

    // Retorna Transacao? para indicar que pode não encontrar
    public Transacao? ObterPorId(Guid id)
    {
        return _transacoes.FirstOrDefault(t => t.Id == id);
    }

    // Exemplo de implementação para métodos futuros (se você adicioná-los na interface)
    // public void Atualizar(Transacao transacao)
    // {
    //      var existingTransacao = _transacoes.FirstOrDefault(t => t.Id == transacao.Id);
    //      if (existingTransacao != null)
    //      {
    //          // Remova e adicione novamente para simular uma atualização completa, ou atualize propriedades individualmente
    //          _transacoes.Remove(existingTransacao);
    //          _transacoes.Add(transacao);
    //      }
    // }

    // public void Remover(Guid id)
    // {
    //      var transacaoToRemove = _transacoes.FirstOrDefault(t => t.Id == id);
    //      if (transacaoToRemove != null)
    //      {
    //          _transacoes.Remove(transacaoToRemove);
    //      }
    // }
}