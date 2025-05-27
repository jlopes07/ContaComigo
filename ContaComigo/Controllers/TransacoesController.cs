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
    private readonly ExcluirTransacao _excluirTransacao;
    private readonly AtualizarTransacao _atualizarTransacao; // NOVO: Injeção do Use Case de atualização

    public TransacoesController(
        RegistrarTransacao registrarTransacao,
        ObterTodasTransacoes obterTodasTransacoes,
        ObterSaldoTotal obterSaldoTotal,
        ExcluirTransacao excluirTransacao,
        AtualizarTransacao atualizarTransacao) // NOVO: Adicionado ao construtor
    {
        _registrarTransacao = registrarTransacao;
        _obterTodasTransacoes = obterTodasTransacoes;
        _obterSaldoTotal = obterSaldoTotal;
        _excluirTransacao = excluirTransacao;
        _atualizarTransacao = atualizarTransacao; // NOVO: Atribuição
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
            Console.WriteLine($"DEBUG (TransacoesController): DataInicio recebida: {filtro.DataInicio?.ToString("yyyy-MM-dd HH:mm:ss.fff") ?? "NULO"}");
            Console.WriteLine($"DEBUG (TransacoesController): DataFim recebida: {filtro.DataFim?.ToString("yyyy-MM-dd HH:mm:ss.fff") ?? "NULO"}");

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

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        Console.WriteLine($"DEBUG (TransacoesController): DELETE /api/transacoes/{id} chamado.");
        try
        {
            var isDeleted = _excluirTransacao.Executar(id);

            if (isDeleted)
            {
                Console.WriteLine($"DEBUG (TransacoesController): Transação com ID {id} excluída com sucesso (HTTP 204).");
                return NoContent();
            }
            else
            {
                Console.WriteLine($"DEBUG (TransacoesController): Transação com ID {id} não encontrada para exclusão (HTTP 404).");
                return NotFound(new { message = $"Transação com ID {id} não encontrada." });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DEBUG (TransacoesController): Erro no DELETE da API: {ex.Message}");
            return StatusCode(500, new { message = "Ocorreu um erro interno ao excluir a transação." });
        }
    }

    // NOVO: Endpoint para atualizar uma transação
    [HttpPut("{id}")] // Define que é um método PUT e espera o ID na rota
    public IActionResult Update(Guid id, [FromBody] TransacaoDto transacaoDto)
    {
        Console.WriteLine($"DEBUG (TransacoesController): PUT /api/transacoes/{id} chamado.");
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Cria um objeto Transacao completo usando o ID da rota e os dados do DTO
            var transacaoAtualizada = new Transacao(
                transacaoDto.Descricao,
                transacaoDto.Valor,
                transacaoDto.Data,
                transacaoDto.Tipo,
                transacaoDto.Categoria
            )
            {
                Id = id // Garante que a transação para atualização tem o ID correto da rota
            };

            var isUpdated = _atualizarTransacao.Executar(transacaoAtualizada); // Chama o Use Case de atualização

            if (isUpdated)
            {
                Console.WriteLine($"DEBUG (TransacoesController): Transação com ID {id} atualizada com sucesso (HTTP 200).");
                return Ok(transacaoAtualizada); // 200 OK - Retorna a transação atualizada
            }
            else
            {
                Console.WriteLine($"DEBUG (TransacoesController): Transação com ID {id} não encontrada para atualização (HTTP 404).");
                return NotFound(new { message = $"Transação com ID {id} não encontrada." }); // 404 Not Found
            }
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message ?? "Dados da transação inválidos." });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DEBUG (TransacoesController): Erro no PUT da API: {ex.Message}");
            return StatusCode(500, new { message = "Ocorreu um erro interno ao atualizar a transação." });
        }
    }
}