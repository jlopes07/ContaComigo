// ContaComigo.Application/UseCases/Transacoes/ObterTodasTransacoes.cs
using ContaComigo.Application.Interfaces;
using ContaComigo.Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ContaComigo.Application.UseCases.Transacoes;

public class ObterTodasTransacoes
{
    private readonly ITransacaoRepository _transacaoRepository;

    public ObterTodasTransacoes(ITransacaoRepository transacaoRepository)
    {
        _transacaoRepository = transacaoRepository;
    }

    public IEnumerable<Transacao> Executar(TransacaoFiltroDto? filtro = null)
    {
        Console.WriteLine($"DEBUG (ObterTodasTransacoes UseCase): Executar chamado.");
        if (filtro != null)
        {
            Console.WriteLine($"DEBUG (ObterTodasTransacoes UseCase): Filtro recebido:");
            Console.WriteLine($"DEBUG (ObterTodasTransacoes UseCase): Tipo: {filtro.Tipo}");
            Console.WriteLine($"DEBUG (ObterTodasTransacoes UseCase): Categoria: {filtro.Categoria}");
            // Log para ver o valor exato de DataInicio e DataFim que o Use Case está trabalhando
            Console.WriteLine($"DEBUG (ObterTodasTransacoes UseCase): DataInicio do filtro: {filtro.DataInicio?.ToString("yyyy-MM-dd HH:mm:ss.fff") ?? "NULO"}");
            Console.WriteLine($"DEBUG (ObterTodasTransacoes UseCase): DataFim do filtro: {filtro.DataFim?.ToString("yyyy-MM-dd HH:mm:ss.fff") ?? "NULO"}");
        }
        else
        {
            Console.WriteLine("DEBUG (ObterTodasTransacoes UseCase): Nenhum filtro recebido (filtro é NULO).");
        }


        var transacoes = _transacaoRepository.GetAll();
        Console.WriteLine($"DEBUG (ObterTodasTransacoes UseCase): Total de transações antes do filtro: {transacoes?.Count() ?? 0}");

        if (filtro != null)
        {
            // Aplica o filtro por Tipo, se fornecido
            if (filtro.Tipo.HasValue)
            {
                transacoes = transacoes.Where(t => t.Tipo == filtro.Tipo.Value);
                Console.WriteLine($"DEBUG (ObterTodasTransacoes UseCase): Aplicando filtro por Tipo: {filtro.Tipo.Value}. Transações restantes: {transacoes.Count()}");
            }

            // Aplica o filtro por Categoria, se fornecido
            if (filtro.Categoria.HasValue)
            {
                transacoes = transacoes.Where(t => t.Categoria == filtro.Categoria.Value);
                Console.WriteLine($"DEBUG (ObterTodasTransacoes UseCase): Aplicando filtro por Categoria: {filtro.Categoria.Value}. Transações restantes: {transacoes.Count()}");
            }

            // Novos filtros por Data
            if (filtro.DataInicio.HasValue)
            {
                // Compara apenas a data (ignora a parte da hora)
                transacoes = transacoes.Where(t => t.Data.Date >= filtro.DataInicio.Value.Date);
                Console.WriteLine($"DEBUG (ObterTodasTransacoes UseCase): Aplicando filtro por DataInicio: {filtro.DataInicio.Value.Date.ToShortDateString()}. Transações restantes: {transacoes.Count()}");
            }
            if (filtro.DataFim.HasValue)
            {
                // Compara apenas a data (ignora a parte da hora).
                transacoes = transacoes.Where(t => t.Data.Date <= filtro.DataFim.Value.Date);
                Console.WriteLine($"DEBUG (ObterTodasTransacoes UseCase): Aplicando filtro por DataFim: {filtro.DataFim.Value.Date.ToShortDateString()}. Transações restantes: {transacoes.Count()}");
            }
        }

        // Ordenar as transações por data, da mais recente para a mais antiga
        var sortedTransacoes = transacoes.OrderByDescending(t => t.Data).ToList(); // .ToList() para materializar e contar
        Console.WriteLine($"DEBUG (ObterTodasTransacoes UseCase): Total de transações após todos os filtros e ordenação: {sortedTransacoes.Count}");
        return sortedTransacoes;
    }
}