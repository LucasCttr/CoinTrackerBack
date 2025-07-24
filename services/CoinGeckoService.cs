using System.Text.Json;
using coninTracker.API.DTOs.CoinGecko;

namespace coninTracker.API.Services;

public class CoinGeckoService : ICoinGeckoService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.coingecko.com/api/v3";
    
    public CoinGeckoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "CoinTrackerApp/1.0");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
    }

    public async Task<List<CoinGeckoDto>> GetCoinsAsync(int page = 1, int pageSize = 20)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"{BaseUrl}/coins/markets?vs_currency=usd&order=market_cap_desc&per_page={pageSize}&page={page}&sparkline=false"
            );
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al obtener datos de CoinGecko: {response.StatusCode} - {errorContent}");
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var coins = JsonSerializer.Deserialize<List<CoinGeckoDto>>(jsonContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            return coins ?? new List<CoinGeckoDto>();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener monedas: {ex.Message}");
        }
    }

    public async Task<CoinGeckoDto> GetCoinDataAsync(string coinId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/coins/{coinId}");
            
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error al obtener datos de {coinId}: {response.StatusCode}");

            var jsonContent = await response.Content.ReadAsStringAsync();
            var coin = JsonSerializer.Deserialize<CoinGeckoDto>(jsonContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            });

            return coin ?? throw new Exception($"No se encontró la moneda {coinId}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener datos de {coinId}: {ex.Message}");
        }
    }
}