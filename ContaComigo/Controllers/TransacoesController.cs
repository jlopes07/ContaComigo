// ContaComigo/Controllers/TransacoesController.cs
using Microsoft.AspNetCore.Mvc;
using ContaComigo.Application.UseCases.Transacoes;
using ContaComigo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContaComigo.Controllers;

[ApiController]
[Route("api/transacoes")]
public class TransacoesController : ControllerBase
{
    private readonly RegistrarTransacao _registrarTransacao;
    private readonly ObterTodasTransacoes _obterTodasTransacoes;
    private readonly ObterSaldoTotal _obterSaldoTotal;

    public TransacoesController(
        RegistrarTransacao registrarTransacao,
        ObterTodasTransacoes obterTodasTransacoes,
        ObterSaldoTotal obterSaldoTotal)
    {
        _registrarTransacao = registrarTransacao;
        _obterTodasTransacoes = obterTodasTransacoes;
        _obterSaldoTotal = obterSaldoTotal;
    }

    [HttpPost]
    public IActionResult Post([FromBody] TransacaoDto transacaoDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var transacao = new Transacao(
                transacaoDto.Descricao,
                transacaoDto.Valor,
                transacaoDto.Data,
                transacaoDto.Tipo,
                transacaoDto.Categoria
            );
            _registrarTransacao.Executar(transacao);
            return CreatedAtAction(nameof(GetById), new { id = transacao.Id }, transacao);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message ?? "Dados da transação inválidos." });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DEBUG (TransacoesController): Erro no POST da API: {ex.Message}");
            return StatusCode(500, new { message = "Ocorreu um erro interno ao registrar a transação." });
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<Transacao>> Get([FromQuery] TransacaoFiltroDto? filtro = null)
    {
        Console.WriteLine($"DEBUG (TransacoesController): GET /api/transacoes chamado.");

        if (filtro != null)
        {
            Console.WriteLine($"DEBUG (TransacoesController): Filtro recebido do cliente:");
            Console.WriteLine($"DEBUG (TransacoesController): Tipo: {filtro.Tipo}");
            Console.WriteLine($"DEBUG (TransacoesController): Categoria: {filtro.Categoria}");
            // Log para ver o valor exato de DataInicio e DataFim recebido no controller
            Console.WriteLine($"DEBUG (TransacoesController): DataInicio recebida: {filtro.DataInicio?.ToString("yyyy-MM-dd HH:mm:ss.fff") ?? "NULO"}");
            Console.WriteLine($"DEBUG (TransacoesController): DataFim recebida: {filtro.DataFim?.ToString("yyyy-MM-dd HH:mm:ss.fff") ?? "NULO"}");

            // Adicionei esta validação para fins de debug, pode ser útil
            if (filtro.DataInicio.HasValue && filtro.DataFim.HasValue && filtro.DataInicio.Value > filtro.DataFim.Value)
            {
                Console.WriteLine("DEBUG (TransacoesController): Validação: DataInicio é posterior a DataFim.");
                return BadRequest("A data de início não pode ser posterior à data de fim.");
            }
        }
        else
        {
            Console.WriteLine("DEBUG (TransacoesController): Nenhum filtro de query string recebido (filtro é NULO).");
        }


        try
        {
            var transacoes = _obterTodasTransacoes.Executar(filtro);
            Console.WriteLine($"DEBUG (TransacoesController): Retornando {transacoes?.Count()} transações com filtro.");
            return Ok(transacoes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DEBUG (TransacoesController): Erro no GET da API: {ex.Message}");
            return StatusCode(500, new { message = "Ocorreu um erro ao obter as transações." });
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Transacao> GetById(Guid id)
    {
        var transacao = _obterTodasTransacoes.Executar().FirstOrDefault(t => t.Id == id);
        if (transacao == null)
        {
            return NotFound();
        }
        return Ok(transacao);
    }

    [HttpGet("saldo")]
    public ActionResult<decimal> GetSaldo()
    {
        try
        {
            var saldo = _obterSaldoTotal.Executar();
            Console.WriteLine($"API GET Saldo: Saldo total: {saldo}");
            return Ok(saldo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no GET Saldo da API: {ex.Message}");
            return StatusCode(500, new { message = "Ocorreu um erro ao obter o saldo total." });
        }
    }
}