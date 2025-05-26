// ContaComigo.Shared/Models/TransacaoFiltroDto.cs
namespace ContaComigo.Shared.Models;

public class TransacaoFiltroDto
{
    // Nullable para permitir que o filtro seja opcional
    public TipoTransacao? Tipo { get; set; }
    public CategoriaTransacao? Categoria { get; set; }

    // Novos campos para filtro de data
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}