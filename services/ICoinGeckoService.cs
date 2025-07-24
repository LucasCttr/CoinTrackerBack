using coninTracker.API.DTOs.CoinGecko;

namespace coninTracker.API.Services;

public interface ICoinGeckoService
{
    Task<List<CoinGeckoDto>> GetCoinsAsync(int page = 1, int perPage = 20);
    Task<CoinGeckoDto> GetCoinDataAsync(string coinId);
}
