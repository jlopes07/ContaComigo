using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ContaComigo.Shared.Models; // <<< AGORA APONTA PARA O PROJETO SHARED
using System;
using System.Collections.Generic; // Certifique-se de que este using existe

namespace ContaComigo.Client.Services
{
    public class TransacaoService
    {
        private readonly HttpClient _httpClient;

        public TransacaoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> RegistrarTransacaoAsync(TransacaoDto transacaoDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/transacoes", transacaoDto);
                response.EnsureSuccessStatusCode(); // Lança exceção se o código de status HTTP for de erro
                return true;
            }
            catch (HttpRequestException)
            {
                // TODO: Adicionar tratamento de erro mais robusto, talvez logar ou mostrar uma mensagem ao usuário
                return false;
            }
        }

        // Método para obter todas as transações - adicionado para o próximo passo
        public async Task<IEnumerable<Transacao>?> ObterTodasTransacoesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/transacoes");
                response.EnsureSuccessStatusCode();
                // Note: Transacao também virá do Shared.Models agora
                return await response.Content.ReadFromJsonAsync<IEnumerable<Transacao>>();
            }
            catch (HttpRequestException)
            {
                // TODO: Adicionar tratamento de erro
                return null;
            }
        }
    }

    // A CLASSE TransacaoDto FOI REMOVIDA DAQUI
    // Ela agora deve estar em ContaComigo.Shared/Models/TransacaoDto.cs
}