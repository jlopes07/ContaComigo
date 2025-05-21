// ContaComigo.Shared/Models/Transacao.cs

using System;
using System.ComponentModel.DataAnnotations; // Para atributos como [Required], [Range], [StringLength]

namespace ContaComigo.Shared.Models
{
    public class Transacao
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(100, ErrorMessage = "A descrição não pode exceder 100 caracteres.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório.")]
        // Alterando o Range para um tipo que o Blazor entenda melhor com decimal
        // Usando Range(0.01M, decimal.MaxValue) se o Range com double estiver causando problemas
        // Ou, para garantir, vamos manter o double.MaxValue como limite superior, mas garantir o tipo decimal no lower bound
        [Range(0.01, (double)20000000000000000000000000000M, ErrorMessage = "O valor deve ser positivo e válido.")] // Ajustei para um valor decimal mais alto e explícito
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "A data é obrigatória.")]
        // [DataType(DataType.Date)] // Pode ser adicionado, mas InputDate já trata isso
        public DateTime Data { get; set; }

        public Transacao()
        {
            Id = Guid.NewGuid();
            Descricao = string.Empty;
            Data = DateTime.Now;
        }

        public Transacao(string descricao, decimal valor, DateTime data)
        {
            Id = Guid.NewGuid();
            Descricao = descricao;
            Valor = valor;
            Data = data;
        }
    }
}