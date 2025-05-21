using System; // Certifique-se de que este using existe

namespace ContaComigo.Shared.Models // <<< MUDE PARA ESTE NAMESPACE
{
    public class TransacaoDto
    {
        public string Descricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
    }
}