// ContaComigo.Infrastructure/Repositories/InMemoryTransacaoRepository.cs
using ContaComigo.Application.Interfaces;
using ContaComigo.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ContaComigo.Infrastructure.Repositories
{
    public class InMemoryTransacaoRepository : ITransacaoRepository
    {
        private static readonly List<Transacao> _transacoes = new List<Transacao>
        {
            // Ajuste aqui para incluir TipoTransacao e CategoriaTransacao
            new Transacao("Aluguel", 1500.00m, new DateTime(2025, 4, 1), TipoTransacao.Saida, CategoriaTransacao.Aluguel), // Valor já deve ser negativo se a lógica do Use Case rodou
            new Transacao("Salário", 3000.00m, new DateTime(2025, 4, 5), TipoTransacao.Entrada, CategoriaTransacao.Salario),
            new Transacao("Compras Supermercado", 250.00m, new DateTime(2025, 4, 10), TipoTransacao.Saida, CategoriaTransacao.Alimentacao) // Valor já deve ser negativo se a lógica do Use Case rodou
        };

        public IEnumerable<Transacao> GetAll()
        {
            return _transacoes.AsEnumerable();
        }

        public void Add(Transacao transacao)
        {
            if (transacao.Id == Guid.Empty)
            {
                transacao.Id = Guid.NewGuid();
            }
            _transacoes.Add(transacao);
        }

        public Transacao? GetById(Guid id)
        {
            return _transacoes.FirstOrDefault(t => t.Id == id);
        }
    }
}