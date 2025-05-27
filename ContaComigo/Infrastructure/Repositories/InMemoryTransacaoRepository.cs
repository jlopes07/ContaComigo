// ContaComigo.Infrastructure/Repositories/InMemoryTransacaoRepository.cs

using ContaComigo.Application.Interfaces;
using ContaComigo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContaComigo.Infrastructure.Repositories;

public class InMemoryTransacaoRepository : ITransacaoRepository
{
    // Usamos uma lista estática para simular o armazenamento em memória
    // persistente entre as requisições de teste (mas cada teste limpa ela com o método Clear na extensão)
    private static readonly List<Transacao> _transacoes = new();

    public InMemoryTransacaoRepository()
    {
        // Adiciona algumas transações iniciais apenas para fins de teste/demonstração,
        // mas testes de integração geralmente limpam isso para garantir um estado controlado.
        // if (!_transacoes.Any())
        // {
        //     _transacoes.Add(new Transacao("Salário", 3000m, DateTime.Now.AddDays(-5), TipoTransacao.Entrada, CategoriaTransacao.Salario));
        //     _transacoes.Add(new Transacao("Aluguel", -1200m, DateTime.Now.AddDays(-3), TipoTransacao.Saida, CategoriaTransacao.Aluguel));
        //     _transacoes.Add(new Transacao("Supermercado", -350m, DateTime.Now.AddDays(-1), TipoTransacao.Saida, CategoriaTransacao.Alimentacao));
        // }
    }

    public IEnumerable<Transacao> GetAll()
    {
        Console.WriteLine($"DEBUG (InMemoryTransacaoRepository): Retornando todas as transações. Total: {_transacoes.Count}");
        foreach (var transacao in _transacoes)
        {
            Console.WriteLine($"DEBUG (InMemoryTransacaoRepository): Transação: Id={transacao.Id}, Desc={transacao.Descricao}, Valor={transacao.Valor}, Data={transacao.Data.ToShortDateString()}, Tipo={transacao.Tipo}, Cat={transacao.Categoria}");
        }
        return _transacoes;
    }

    public Transacao GetById(Guid id)
    {
        return _transacoes.FirstOrDefault(t => t.Id == id);
    }

    public void Add(Transacao transacao)
    {
        // Garante que o valor da transação de saída seja sempre negativo ao armazenar
        if (transacao.Tipo == TipoTransacao.Saida && transacao.Valor > 0)
        {
            transacao.Valor = -transacao.Valor;
        }
        _transacoes.Add(transacao);
        Console.WriteLine($"DEBUG (InMemoryTransacaoRepository): Adicionada transação: Id={transacao.Id}, Desc={transacao.Descricao}, Valor={transacao.Valor}, Data={transacao.Data.ToShortDateString()}, Tipo={transacao.Tipo}, Cat={transacao.Categoria}");
    }

    public void Update(Transacao transacao)
    {
        var existingTransacao = _transacoes.FirstOrDefault(t => t.Id == transacao.Id);
        if (existingTransacao != null)
        {
            // Garante que o valor da transação de saída seja sempre negativo ao atualizar
            if (transacao.Tipo == TipoTransacao.Saida && transacao.Valor > 0)
            {
                transacao.Valor = -transacao.Valor;
            }

            existingTransacao.Descricao = transacao.Descricao;
            existingTransacao.Valor = transacao.Valor;
            existingTransacao.Data = transacao.Data;
            existingTransacao.Tipo = transacao.Tipo;
            existingTransacao.Categoria = transacao.Categoria;
            Console.WriteLine($"DEBUG (InMemoryTransacaoRepository): Atualizada transação: Id={transacao.Id}");
        }
        else
        {
            Console.WriteLine($"DEBUG (InMemoryTransacaoRepository): Tentativa de atualizar transação não existente: Id={transacao.Id}");
        }
    }

    // NOVO: Implementação do método Delete
    public bool Delete(Guid id)
    {
        var transacaoToRemove = _transacoes.FirstOrDefault(t => t.Id == id);
        if (transacaoToRemove != null)
        {
            _transacoes.Remove(transacaoToRemove);
            Console.WriteLine($"DEBUG (InMemoryTransacaoRepository): Transação com ID {id} removida com sucesso.");
            return true;
        }
        Console.WriteLine($"DEBUG (InMemoryTransacaoRepository): Transação com ID {id} não encontrada no repositório para remoção.");
        return false;
    }
}