using System;
using System.Collections.Generic; // Certifique-se de ter este using para IEnumerable, se usado

namespace ContaComigo.Shared.Models;

public class Transacao
{
    public Guid Id { get; private set; }
    public string Descricao { get; private set; }
    public decimal Valor { get; private set; }
    public DateTime Data { get; private set; }

    public Transacao(string descricao, decimal valor, DateTime data)
    {
        // Validações básicas da entidade (se for o caso)
        if (string.IsNullOrWhiteSpace(descricao))
        {
            throw new ArgumentException("A descrição da transação não pode ser nula ou vazia.", nameof(descricao));
        }
        if (valor <= 0)
        {
            throw new ArgumentException("O valor da transação deve ser maior que zero.", nameof(valor));
        }

        Id = Guid.NewGuid();
        Descricao = descricao;
        Valor = valor;
        Data = data;
    }

    // Construtor privado para deserialização/ORM, inicializando propriedades não anuláveis
    private Transacao()
    {
        Id = Guid.Empty;
        Descricao = string.Empty; // Inicialização para evitar CS8618
        Valor = 0m;
        Data = DateTime.MinValue;
    }

    // Métodos de comportamento da entidade (ex: atualizar, mas com validação)
    public void AtualizarDescricao(string novaDescricao)
    {
        if (string.IsNullOrWhiteSpace(novaDescricao))
        {
            throw new ArgumentException("A descrição não pode ser nula ou vazia.", nameof(novaDescricao));
        }
        Descricao = novaDescricao;
    }

    public void AtualizarValor(decimal novoValor)
    {
        if (novoValor <= 0)
        {
            throw new ArgumentException("O valor deve ser maior que zero.", nameof(novoValor));
        }
        Valor = novoValor;
    }
}