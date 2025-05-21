using System;
using System.ComponentModel.DataAnnotations;

namespace ContaComigo.Shared.Models
{
    public class TransacaoDto
    {
        [Required(ErrorMessage = "A descrição é obrigatória.")]
        public string Descricao { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser positivo e válido.")]
        public decimal Valor { get; set; }

        public DateTime Data { get; set; }
    }
}
