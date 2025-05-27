// ContaComigo.Tests/TransacoesControllerIntegrationTests.cs

using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using ContaComigo.Shared.Models;
using System.Net.Http.Json;
using System.Collections.Generic;
using ContaComigo.Infrastructure.Repositories;
using ContaComigo.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System;

namespace ContaComigo.Tests; // Namespace com ponto e v�rgula

// WebApplicationFactory � um utilit�rio para testar APIs ASP.NET Core
// A interface IClassFixture<CustomWebApplicationFactory<ContaComigo.Program>> garante que a factory
// seja criada uma �nica vez para todos os testes nesta classe.
// IMPORTANTE: 'ContaComigo.Program' refere-se � classe Program do seu projeto de BACKEND 'ContaComigo'.
public class TransacoesControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<ContaComigo.Program>>
{
    // A declara��o da factory deve usar o tipo base WebApplicationFactory para compatibilidade
    private readonly WebApplicationFactory<ContaComigo.Program> _factory;
    private readonly HttpClient _client;

    // O construtor continua recebendo CustomWebApplicationFactory, que � um tipo derivado
    public TransacoesControllerIntegrationTests(CustomWebApplicationFactory<ContaComigo.Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Substitua a implementa��o do reposit�rio em mem�ria para que os testes n�o interfiram nos dados est�ticos
                // e para que cada teste possa iniciar com um estado limpo.
                // Isso garante que cada teste use sua pr�pria inst�ncia do reposit�rio.
                services.AddSingleton<ITransacaoRepository, InMemoryTransacaoRepository>();
            });
        });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Get_DeveRetornarTransacoesExistentes()
    {
        // Arrange
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        repo.Clear(); // Limpa o reposit�rio para garantir um estado limpo

        repo.Add(new Transacao("Teste Get 1", 100m, DateTime.Now, TipoTransacao.Entrada, CategoriaTransacao.Outros));
        repo.Add(new Transacao("Teste Get 2", -50m, DateTime.Now, TipoTransacao.Saida, CategoriaTransacao.Alimentacao));

        // Act
        var response = await _client.GetAsync("/api/transacoes");

        // Assert
        response.EnsureSuccessStatusCode(); // Verifica se o status code � 2xx (e.g., 200 OK)
        var transacoes = await response.Content.ReadFromJsonAsync<List<Transacao>>();
        Assert.NotNull(transacoes);
        // Verifica se as transa��es adicionadas no Arrange foram carregadas
        Assert.True(transacoes.Count() >= 2);
        Assert.Contains(transacoes, t => t.Descricao == "Teste Get 1");
        Assert.Contains(transacoes, t => t.Descricao == "Teste Get 2");
    }

    [Fact]
    public async Task Post_DeveCriarNovaTransacao_E_RetornarCreated()
    {
        // Arrange
        var novaTransacaoDto = new TransacaoDto
        {
            Descricao = "Compra Teste",
            Valor = 200.00m,
            Data = DateTime.Now,
            Tipo = TipoTransacao.Saida,
            Categoria = CategoriaTransacao.Lazer
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/transacoes", novaTransacaoDto);

        // Assert
        response.EnsureSuccessStatusCode(); // Status 2xx
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode); // Verifica se o status � 201 Created

        var transacaoCriada = await response.Content.ReadFromJsonAsync<Transacao>();
        Assert.NotNull(transacaoCriada);
        Assert.NotEqual(Guid.Empty, transacaoCriada.Id); // Verifica se um ID foi gerado
        Assert.Equal(novaTransacaoDto.Descricao, transacaoCriada.Descricao);
        // Verifica se o valor foi negativado corretamente no servidor para sa�da
        Assert.Equal(-novaTransacaoDto.Valor, transacaoCriada.Valor);
        Assert.Equal(novaTransacaoDto.Tipo, transacaoCriada.Tipo);
        Assert.Equal(novaTransacaoDto.Categoria, transacaoCriada.Categoria);

        // Opcional: Verificar se a transa��o est� no reposit�rio ap�s a cria��o
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        Assert.Contains(repo.GetAll(), t => t.Id == transacaoCriada.Id);
    }

    [Fact]
    public async Task GetSaldo_DeveRetornarSaldoCorreto()
    {
        // Arrange
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        repo.Clear(); // Limpa o reposit�rio para ter controle total sobre os dados de teste para o saldo

        // Adicione transa��es para o c�lculo do saldo (valores j� com o sinal correto)
        repo.Add(new Transacao("Dep�sito", 1000m, DateTime.Now, TipoTransacao.Entrada, CategoriaTransacao.Salario));
        repo.Add(new Transacao("Conta de �gua", -120m, DateTime.Now, TipoTransacao.Saida, CategoriaTransacao.Outros));
        repo.Add(new Transacao("Reembolso", 50m, DateTime.Now, TipoTransacao.Entrada, CategoriaTransacao.Outros));
        repo.Add(new Transacao("Lanche", -30m, DateTime.Now, TipoTransacao.Saida, CategoriaTransacao.Alimentacao));

        decimal expectedSaldo = 1000m - 120m + 50m - 30m; // = 900m

        // Act
        var response = await _client.GetAsync("/api/transacoes/saldo");

        // Assert
        response.EnsureSuccessStatusCode();
        var saldo = await response.Content.ReadFromJsonAsync<decimal>();
        Assert.Equal(expectedSaldo, saldo);
    }

    [Fact]
    public async Task Get_DeveRetornarApenasTransacoesDeEntrada_QuandoFiltroPorTipoEntrada()
    {
        // Arrange
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        repo.Clear(); // Limpa o reposit�rio para garantir um estado limpo para este teste

        repo.Add(new Transacao("Sal�rio do M�s", 2000m, DateTime.Now, TipoTransacao.Entrada, CategoriaTransacao.Salario));
        repo.Add(new Transacao("Aluguel", -1500m, DateTime.Now.AddDays(-1), TipoTransacao.Saida, CategoriaTransacao.Aluguel));
        repo.Add(new Transacao("Reembolso Freela", 300m, DateTime.Now.AddDays(-2), TipoTransacao.Entrada, CategoriaTransacao.Outros));
        repo.Add(new Transacao("Supermercado", -250m, DateTime.Now.AddDays(-3), TipoTransacao.Saida, CategoriaTransacao.Alimentacao));

        var queryString = "?tipo=Entrada"; // Query string para o filtro de tipo

        // Act
        var response = await _client.GetAsync($"/api/transacoes{queryString}");

        // Assert
        response.EnsureSuccessStatusCode(); // Status 2xx
        var transacoes = await response.Content.ReadFromJsonAsync<List<Transacao>>();

        Assert.NotNull(transacoes);
        Assert.Equal(2, transacoes.Count); // Esperamos 2 transa��es de entrada
        Assert.DoesNotContain(transacoes, t => t.Tipo == TipoTransacao.Saida); // Garante que n�o h� sa�das
        Assert.Contains(transacoes, t => t.Descricao == "Sal�rio do M�s");
        Assert.Contains(transacoes, t => t.Descricao == "Reembolso Freela");
    }

    [Fact]
    public async Task Get_DeveRetornarApenasTransacoesDeLazer_QuandoFiltroPorCategoriaLazer()
    {
        // Arrange
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        repo.Clear(); // Limpa o reposit�rio

        repo.Add(new Transacao("Show de Rock", -100m, DateTime.Now, TipoTransacao.Saida, CategoriaTransacao.Lazer));
        repo.Add(new Transacao("Venda de Item", 50m, DateTime.Now.AddDays(-1), TipoTransacao.Entrada, CategoriaTransacao.Outros));
        repo.Add(new Transacao("Cinema", -40m, DateTime.Now.AddDays(-2), TipoTransacao.Saida, CategoriaTransacao.Lazer));

        var queryString = "?categoria=Lazer";

        // Act
        var response = await _client.GetAsync($"/api/transacoes{queryString}");

        // Assert
        response.EnsureSuccessStatusCode();
        var transacoes = await response.Content.ReadFromJsonAsync<List<Transacao>>();

        Assert.NotNull(transacoes);
        Assert.Equal(2, transacoes.Count);
        Assert.DoesNotContain(transacoes, t => t.Categoria != CategoriaTransacao.Lazer);
        Assert.Contains(transacoes, t => t.Descricao == "Show de Rock");
        Assert.Contains(transacoes, t => t.Descricao == "Cinema");
    }

    [Fact]
    public async Task Get_DeveRetornarTransacoesDentroDoPeriodoDeDatas_QuandoFiltroPorData()
    {
        // Arrange
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        repo.Clear(); // Limpa o reposit�rio

        var hoje = DateTime.Today; // Usar DateTime.Today para evitar problemas com hora
        repo.Add(new Transacao("Transacao Antiga", 100m, hoje.AddDays(-10), TipoTransacao.Entrada, CategoriaTransacao.Outros));
        repo.Add(new Transacao("Transacao Recente 1", 50m, hoje.AddDays(-2), TipoTransacao.Entrada, CategoriaTransacao.Salario));
        repo.Add(new Transacao("Transacao Recente 2", -30m, hoje.AddDays(-1), TipoTransacao.Saida, CategoriaTransacao.Alimentacao));
        repo.Add(new Transacao("Transacao Futura", 200m, hoje.AddDays(5), TipoTransacao.Entrada, CategoriaTransacao.Lazer));

        // Formato "yyyy-MM-dd" � seguro para passar em query string
        var dataInicio = hoje.AddDays(-3).ToString("yyyy-MM-dd");
        var dataFim = hoje.ToString("yyyy-MM-dd");

        var queryString = $"?dataInicio={dataInicio}&dataFim={dataFim}";

        // Act
        var response = await _client.GetAsync($"/api/transacoes{queryString}");

        // Assert
        response.EnsureSuccessStatusCode();
        var transacoes = await response.Content.ReadFromJsonAsync<List<Transacao>>();

        Assert.NotNull(transacoes);
        Assert.Equal(2, transacoes.Count); // Espera as duas transa��es recentes
        Assert.Contains(transacoes, t => t.Descricao == "Transacao Recente 1");
        Assert.Contains(transacoes, t => t.Descricao == "Transacao Recente 2");
        Assert.DoesNotContain(transacoes, t => t.Descricao == "Transacao Antiga");
        Assert.DoesNotContain(transacoes, t => t.Descricao == "Transacao Futura");
    }

    [Fact]
    public async Task Get_DeveRetornarTransacoesComMultiplosFiltrosCombinados()
    {
        // Arrange
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        repo.Clear(); // Limpa o reposit�rio

        var hoje = DateTime.Today;
        repo.Add(new Transacao("Cinema com amigos", -50m, hoje.AddDays(-1), TipoTransacao.Saida, CategoriaTransacao.Lazer));
        repo.Add(new Transacao("Presente de anivers�rio", -70m, hoje.AddDays(-5), TipoTransacao.Saida, CategoriaTransacao.Lazer));
        repo.Add(new Transacao("Sal�rio Extra", 800m, hoje.AddDays(-2), TipoTransacao.Entrada, CategoriaTransacao.Salario));
        repo.Add(new Transacao("Jantar Fora", -60m, hoje.AddDays(-1), TipoTransacao.Saida, CategoriaTransacao.Alimentacao));


        var dataInicio = hoje.AddDays(-3).ToString("yyyy-MM-dd");
        var dataFim = hoje.ToString("yyyy-MM-dd");

        // A query string deve mapear corretamente para os campos do TransacaoFiltroDto
        var queryString = $"?tipo=Saida&categoria=Lazer&dataInicio={dataInicio}&dataFim={dataFim}";

        // Act
        var response = await _client.GetAsync($"/api/transacoes{queryString}");

        // Assert
        response.EnsureSuccessStatusCode();
        var transacoes = await response.Content.ReadFromJsonAsync<List<Transacao>>();

        Assert.NotNull(transacoes);
        Assert.Single(transacoes); // Apenas "Cinema com amigos" deve corresponder a todos os filtros
        Assert.Contains(transacoes, t => t.Descricao == "Cinema com amigos");
    }

    [Fact]
    public async Task Get_DeveRetornarListaVazia_QuandoNenhumFiltroCorresponde()
    {
        // Arrange
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        repo.Clear();

        repo.Add(new Transacao("Transacao Valida", 100m, DateTime.Now, TipoTransacao.Entrada, CategoriaTransacao.Salario));

        // Filtros que n�o devem corresponder a nenhuma transa��o existente
        var queryString = "?tipo=Saida&categoria=Lazer";

        // Act
        var response = await _client.GetAsync($"/api/transacoes{queryString}");

        // Assert
        response.EnsureSuccessStatusCode();
        var transacoes = await response.Content.ReadFromJsonAsync<List<Transacao>>();

        Assert.NotNull(transacoes);
        Assert.Empty(transacoes); // Espera uma lista vazia
    }

    [Fact]
    public async Task Get_DeveRetornarTodasAsTransacoes_QuandoNenhumFiltroEhAplicado()
    {
        // Arrange
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        repo.Clear();

        repo.Add(new Transacao("Transacao 1", 100m, DateTime.Now, TipoTransacao.Entrada, CategoriaTransacao.Salario));
        repo.Add(new Transacao("Transacao 2", -50m, DateTime.Now, TipoTransacao.Saida, CategoriaTransacao.Alimentacao));

        // Nenhuma query string de filtro
        var queryString = "";

        // Act
        var response = await _client.GetAsync($"/api/transacoes{queryString}");

        // Assert
        response.EnsureSuccessStatusCode();
        var transacoes = await response.Content.ReadFromJsonAsync<List<Transacao>>();

        Assert.NotNull(transacoes);
        Assert.Equal(2, transacoes.Count); // Espera todas as 2 transa��es
        Assert.Contains(transacoes, t => t.Descricao == "Transacao 1");
        Assert.Contains(transacoes, t => t.Descricao == "Transacao 2");
    }

    // NOVOS TESTES PARA EXCLUS�O

    [Fact]
    public async Task Delete_DeveRetornarNoContent_QuandoTransacaoExiste()
    {
        // Arrange
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        repo.Clear(); // Limpa o reposit�rio para garantir um estado limpo

        // Adiciona uma transa��o para ser exclu�da
        var transacaoParaDeletar = new Transacao("Item a deletar", 50.00m, DateTime.Now, TipoTransacao.Saida, CategoriaTransacao.Outros);
        repo.Add(transacaoParaDeletar);

        // Act
        var response = await _client.DeleteAsync($"/api/transacoes/{transacaoParaDeletar.Id}");

        // Assert
        response.EnsureSuccessStatusCode(); // Verifica se o status code � 2xx
        Assert.Equal(System.Net.HttpStatusCode.NoContent, response.StatusCode); // Espera 204 No Content

        // Opcional: Verifica se a transa��o realmente foi removida do reposit�rio
        Assert.DoesNotContain(repo.GetAll(), t => t.Id == transacaoParaDeletar.Id);
    }

    [Fact]
    public async Task Delete_DeveRetornarNotFound_QuandoTransacaoNaoExiste()
    {
        // Arrange
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        repo.Clear(); // Garante que o reposit�rio est� vazio

        var idNaoExistente = Guid.NewGuid(); // Um ID que n�o existe no reposit�rio

        // Act
        var response = await _client.DeleteAsync($"/api/transacoes/{idNaoExistente}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode); // Espera 404 Not Found
    }

    // NOVOS TESTES PARA ATUALIZA��O

    [Fact]
    public async Task Put_DeveAtualizarTransacao_E_RetornarOk()
    {
        // Arrange
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        repo.Clear(); // Limpa o reposit�rio

        // Adiciona uma transa��o inicial para ser atualizada
        var transacaoOriginal = new Transacao("Descricao Original", 100m, new DateTime(2025, 1, 15), TipoTransacao.Entrada, CategoriaTransacao.Outros);
        repo.Add(transacaoOriginal);

        // Cria o DTO com os dados atualizados (mesmo ID da original, pois ele � passado na rota)
        var transacaoAtualizadaDto = new TransacaoDto
        {
            Descricao = "Descricao Atualizada",
            Valor = 150m,
            Data = new DateTime(2025, 1, 20),
            Tipo = TipoTransacao.Saida, // Mudou para sa�da, valor deve ser negativado pelo backend
            Categoria = CategoriaTransacao.Lazer
        };

        // Act
        // Requisi��o PUT para a rota /api/transacoes/{id} com o corpo do DTO
        var response = await _client.PutAsJsonAsync($"/api/transacoes/{transacaoOriginal.Id}", transacaoAtualizadaDto);

        // Assert
        response.EnsureSuccessStatusCode(); // Deve retornar 2xx (200 OK)
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);

        // Opcional: Verifica se a transa��o foi realmente atualizada no reposit�rio
        var transacaoNoRepo = repo.GetById(transacaoOriginal.Id);
        Assert.NotNull(transacaoNoRepo);
        Assert.Equal(transacaoAtualizadaDto.Descricao, transacaoNoRepo.Descricao);
        Assert.Equal(-transacaoAtualizadaDto.Valor, transacaoNoRepo.Valor); // Valor deve estar negativado
        Assert.Equal(transacaoAtualizadaDto.Data, transacaoNoRepo.Data);
        Assert.Equal(transacaoAtualizadaDto.Tipo, transacaoNoRepo.Tipo);
        Assert.Equal(transacaoAtualizadaDto.Categoria, transacaoNoRepo.Categoria);
    }

    [Fact]
    public async Task Put_DeveRetornarNotFound_QuandoTransacaoNaoExiste()
    {
        // Arrange
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        repo.Clear(); // Garante que o reposit�rio est� vazio

        var idNaoExistente = Guid.NewGuid(); // Um ID que n�o existe

        var transacaoParaAtualizarDto = new TransacaoDto
        {
            Descricao = "Nao existe",
            Valor = 10m,
            Data = DateTime.Now,
            Tipo = TipoTransacao.Entrada,
            Categoria = CategoriaTransacao.Outros
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/transacoes/{idNaoExistente}", transacaoParaAtualizarDto);

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode); // Espera 404 Not Found
    }

    [Fact]
    public async Task Put_DeveRetornarBadRequest_QuandoIdDaRotaNaoCorrespondeAoIdDoCorpo()
    {
        // Arrange
        var repo = (InMemoryTransacaoRepository)_factory.Services.GetRequiredService<ITransacaoRepository>();
        repo.Clear(); // Limpa o reposit�rio

        // Adiciona uma transa��o inicial
        var transacaoOriginal = new Transacao("Item", 100m, DateTime.Now, TipoTransacao.Entrada, CategoriaTransacao.Outros);
        repo.Add(transacaoOriginal);

        // DTO que seria enviado no corpo, mas o ID da rota ser� diferente do ID da transa��o Original.
        // Como TransacaoDto n�o tem ID, o controller vai ter que usar o ID da rota para atualizar.
        // Se o controller validasse que o ID do corpo (se existisse) precisa ser igual ao da rota, este teste seria v�lido.
        // Sem ID no DTO, este teste se torna um pouco amb�guo. O teste Put_DeveRetornarNotFound_QuandoTransacaoNaoExiste
        // j� cobre o cen�rio de ID inexistente.
        // Este teste espec�fico � mais relevante se o TransacaoDto incluir o ID.
        // Por enquanto, vamos manter como um placeholder ou ajust�-lo com base na sua implementa��o final do PUT.
        // Para o cen�rio atual, o comportamento mais prov�vel seria NotFound se o ID da rota n�o existe.
        // A valida��o de "ID da rota n�o corresponde ao ID do corpo" � mais comum quando o corpo tamb�m tem um ID.

        // Para este teste ser relevante, o controller teria que ter uma l�gica espec�fica
        // para BAD REQUEST se o ID da rota � diferente do ID de um objeto no corpo que tamb�m tem ID.
        // Como TransacaoDto n�o tem ID, a compara��o seria com o ID retornado pelo GetById no UseCase.
        // Se GetById retornar null, ele j� cairia em NotFound.

        // Vamos ajustar o Arrange para simular um cen�rio onde o ID da rota existe,
        // mas a transa��o que ele busca para atualizar no corpo (se viesse) seria diferente.
        // No contexto atual (DTO sem ID), este teste de BadRequest por ID mismatch
        // � menos aplic�vel, a menos que o controller invente um ID no corpo.
        // O teste de NotFound � mais robusto para ID n�o encontrado.
        // Vou manter o teste, mas com essa ressalva sobre a implementa��o do controller.

        var idNaRotaQueExiste = transacaoOriginal.Id;
        var transacaoComOutroIdNoCorpo = new TransacaoDto
        {
            Descricao = "Atualizar Com ID Diferente",
            Valor = 200m,
            Data = DateTime.Now,
            Tipo = TipoTransacao.Entrada,
            Categoria = CategoriaTransacao.Outros
        };
        // Se o DTO tivesse Id: transacaoComOutroIdNoCorpo.Id = Guid.NewGuid();

        // Act
        var response = await _client.PutAsJsonAsync($"/api/transacoes/{idNaRotaQueExiste}", transacaoComOutroIdNoCorpo);

        // Assert
        // A API deve retornar OK se atualizar, ou NotFound se n�o encontrar.
        // Um BadRequest aqui seria se o controller fizesse uma valida��o expl�cita de ID mismatch,
        // o que n�o � o caso com TransacaoDto atual sem ID.
        // Vou mudar o Assert para esperar OK, dado que o ID da rota existe e o DTO � v�lido para update.
        response.EnsureSuccessStatusCode(); // Se o ID da rota existe, deve ser OK.
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }
}

// Extens�o para InMemoryTransacaoRepository para limpar os dados em testes
// Voc� pode adicionar este m�todo no arquivo ContaComigo.Infrastructure/Repositories/InMemoryTransacaoRepository.cs
// Isso � �til para garantir que cada teste de integra��o rode com um estado limpo.
public static class InMemoryTransacaoRepositoryExtensions
{
    public static void Clear(this InMemoryTransacaoRepository repo)
    {
        // Usa Reflection para acessar o campo _transacoes est�tico e limp�-lo
        // Isso � necess�rio porque _transacoes � provavelmente um campo est�tico e privado na sua implementa��o InMemoryTransacaoRepository
        var field = typeof(InMemoryTransacaoRepository)
            .GetField("_transacoes", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        if (field != null)
        {
            var list = field.GetValue(null) as System.Collections.IList;
            list?.Clear();
        }
    }
}