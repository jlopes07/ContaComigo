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
        // Alterado para um List<Transacao> privado para ter controle de instâncias nos testes
        // No entanto, para que o Singleton funcione como um "banco de dados" em memória compartilhado,
        // ele deve ser 'static'. Se você limpa isso nos testes de integração, isso afeta todos.
        // Mantenho como static, mas esteja ciente que a limpeza afeta globalmente no runtime.
        private static readonly List<Transacao> _transacoes = new List<Transacao>
        {
            // Ajuste aqui para incluir TipoTransacao e CategoriaTransacao
            // Garanta que essas datas estão no range que você está testando!
            new Transacao("Aluguel", 1500.00m, new DateTime(2025, 4, 1), TipoTransacao.Saida, CategoriaTransacao.Aluguel),
            new Transacao("Salário", 3000.00m, new DateTime(2025, 4, 5), TipoTransacao.Entrada, CategoriaTransacao.Salario),
            new Transacao("Compras Supermercado", 250.00m, new DateTime(2025, 4, 10), TipoTransacao.Saida, CategoriaTransacao.Alimentacao),
            new Transacao("Lanche", 30.00m, new DateTime(2025, 5, 20), TipoTransacao.Saida, CategoriaTransacao.Alimentacao), // Exemplo maio
            new Transacao("Presente Aniversário", 100.00m, new DateTime(2025, 5, 25), TipoTransacao.Saida, CategoriaTransacao.Lazer), // Exemplo maio
            new Transacao("Receita Extra", 500.00m, new DateTime(2025, 5, 26), TipoTransacao.Entrada, CategoriaTransacao.Outros), // Exemplo maio (hoje)
            new Transacao("Conta de Telefone", 80.00m, new DateTime(2025, 6, 1), TipoTransacao.Saida, CategoriaTransacao.Outros) // Exemplo junho
        };

        public IEnumerable<Transacao> GetAll()
        {
            Console.WriteLine($"DEBUG (InMemoryTransacaoRepository): Retornando todas as transações. Total: {_transacoes.Count}");
            foreach (var t in _transacoes)
            {
                Console.WriteLine($"DEBUG (InMemoryTransacaoRepository): Transação: Id={t.Id}, Desc={t.Descricao}, Valor={t.Valor}, Data={t.Data.ToShortDateString()}, Tipo={t.Tipo}, Cat={t.Categoria}");
            }
            return _transacoes.AsEnumerable();
        }

        public void Add(Transacao transacao)
        {
            if (transacao.Id == Guid.Empty)
            {
                transacao.Id = Guid.NewGuid();
            }
            _transacoes.Add(transacao);
            Console.WriteLine($"DEBUG (InMemoryTransacaoRepository): Adicionada transação: Id={transacao.Id}, Desc={transacao.Descricao}, Valor={transacao.Valor}, Data={transacao.Data.ToShortDateString()}, Tipo={transacao.Tipo}, Cat={transacao.Categoria}");
        }

        public Transacao? GetById(Guid id)
        {
            var found = _transacoes.FirstOrDefault(t => t.Id == id);
            Console.WriteLine($"DEBUG (InMemoryTransacaoRepository): Buscando transação por Id '{id}'. Encontrado: {(found != null ? "Sim" : "Não")}");
            return found;
        }

        // Método para limpar as transações em memória. Útil para testes.
        public static void ClearData()
        {
            _transacoes.Clear();
            Console.WriteLine("DEBUG (InMemoryTransacaoRepository): Dados em memória limpos.");
        }
    }
}