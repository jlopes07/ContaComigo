// ContaComigo.Shared/Models/Transacao.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ContaComigo.Shared.Models
{
    public class Transacao
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(100, ErrorMessage = "A descrição não pode ter mais de 100 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor deve ser positivo e válido.")] // Agora o valor digitado é sempre positivo
        public decimal Valor { get; set; } // Valor digitado pelo usuário (sempre positivo)

        [Required(ErrorMessage = "A data é obrigatória.")]
        public DateTime Data { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "O tipo de transação é obrigatório.")]
        public TipoTransacao Tipo { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public CategoriaTransacao Categoria { get; set; }

        // Construtor sem parâmetros para desserialização
        public Transacao() { }

        // Construtor para facilitar a criação de transações
        public Transacao(string descricao, decimal valor, DateTime data, TipoTransacao tipo, CategoriaTransacao categoria)
        {
            Descricao = descricao;
            Valor = valor;
            Data = data;
            Tipo = tipo;
            Categoria = categoria;
        }
    }
}