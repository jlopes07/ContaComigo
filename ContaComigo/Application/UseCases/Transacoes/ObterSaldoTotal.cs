// ContaComigo/Application/UseCases/Transacoes/ObterSaldoTotal.cs
using ContaComigo.Application.Interfaces;
using System.Linq; // Para usar .Sum()

namespace ContaComigo.Application.UseCases.Transacoes;

public class ObterSaldoTotal
{
    private readonly ITransacaoRepository _transacaoRepository;

    public ObterSaldoTotal(ITransacaoRepository transacaoRepository)
    {
        _transacaoRepository = transacaoRepository;
    }

    public decimal Executar()
    {
        // Obtém todas as transações e soma seus valores.
        // Os valores já devem estar com o sinal correto (positivo para entrada, negativo para saída).
        return _transacaoRepository.GetAll().Sum(t => t.Valor);
    }
}