// ContaComigo.Shared/Models/TransacaoDto.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ContaComigo.Shared.Models
{
    public class TransacaoDto
    {
        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(100, ErrorMessage = "A descrição não pode ter mais de 100 caracteres.")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O valor deve ser positivo e válido.")] // Valor digitado é sempre positivo
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "A data é obrigatória.")]
        public DateTime Data { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "O tipo de transação é obrigatório.")]
        public TipoTransacao Tipo { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        public CategoriaTransacao Categoria { get; set; }
    }
}