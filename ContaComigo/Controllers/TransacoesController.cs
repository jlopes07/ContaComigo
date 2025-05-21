using Microsoft.AspNetCore.Mvc;
using ContaComigo.Application.UseCases.Transacoes;
using ContaComigo.Shared.Models; // Já está correto aqui
// REMOVA ESTA LINHA: using ContaComigo.Client.Services; // Não é mais necessário para TransacaoDto
using System;
using System.Collections.Generic;

namespace ContaComigo.Controllers
{
    [ApiController]
    [Route("api/transacoes")]
    public class TransacoesController : ControllerBase
    {
        private readonly RegistrarTransacao _registrarTransacao;
        private readonly ObterTodasTransacoes _obterTodasTransacoes;

        public TransacoesController(RegistrarTransacao registrarTransacao, ObterTodasTransacoes obterTodasTransacoes)
        {
            _registrarTransacao = registrarTransacao;
            _obterTodasTransacoes = obterTodasTransacoes;
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
                // Mapear DTO para Entidade de Domínio
                // A classe Transacao aqui JÁ ESTÁ usando ContaComigo.Shared.Models.Transacao
                var transacao = new Transacao(transacaoDto.Descricao, transacaoDto.Valor, transacaoDto.Data);
                _registrarTransacao.Executar(transacao); // Aqui é onde o erro ocorre, a 'RegistrarTransacao' espera o tipo certo
                return CreatedAtAction(nameof(GetById), new { id = transacao.Id }, transacao);
            }
            catch (ArgumentException)
            {
                return BadRequest(new { message = "Dados da transação inválidos." });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ocorreu um erro interno ao registrar a transação." });
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Transacao>> Get()
        {
            try
            {
                var transacoes = _obterTodasTransacoes.Executar();
                return Ok(transacoes);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Ocorreu um erro ao obter as transações." });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Transacao> GetById(Guid id)
        {
            return NotFound();
        }
    }
}