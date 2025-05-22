using Microsoft.AspNetCore.Mvc;
using ContaComigo.Application.UseCases.Transacoes;
using ContaComigo.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ContaComigo.Controllers
{
    [ApiController]
    [Route("api/transacoes")]
    public class TransacoesController : ControllerBase
    {
        private readonly RegistrarTransacao _registrarTransacao;
        private readonly ObterTodasTransacoes _obterTodasTransacoes;
        private readonly ObterSaldoTotal _obterSaldoTotal; // Adicionado para o novo Use Case de Saldo

        public TransacoesController(
            RegistrarTransacao registrarTransacao,
            ObterTodasTransacoes obterTodasTransacoes,
            ObterSaldoTotal obterSaldoTotal) // Injetando o novo Use Case
        {
            _registrarTransacao = registrarTransacao;
            _obterTodasTransacoes = obterTodasTransacoes;
            _obterSaldoTotal = obterSaldoTotal; // Atribuição
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
                // Mapear DTO para Entidade de Domínio (Transacao) com todos os novos campos
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
                Console.WriteLine($"Erro no POST da API: {ex.Message}");
                return StatusCode(500, new { message = "Ocorreu um erro interno ao registrar a transação." });
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Transacao>> Get()
        {
            try
            {
                var transacoes = _obterTodasTransacoes.Executar();
                Console.WriteLine($"API GET: Retornando {transacoes?.Count()} transações.");
                return Ok(transacoes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no GET da API: {ex.Message}");
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

        // Novo endpoint para obter o saldo total
        [HttpGet("saldo")] // Rota específica para o saldo, ex: /api/transacoes/saldo
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
}