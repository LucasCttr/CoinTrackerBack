using coninTracker.API.Services;
using coninTracker.API.DTOs.CoinGecko;
using coninTracker.API.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace coninTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoinsController : ControllerBase
{
    private readonly ICoinGeckoService _coinGeckoService;

    public CoinsController(ICoinGeckoService coinGeckoService)
    {
        _coinGeckoService = coinGeckoService;
    }


    [HttpGet]
    public async Task<IActionResult> GetCoins(
        string? cursor = null, 
        int limit = 20)
    {
        try
        {
            // Convertir cursor a página
            int page = 1;
            if (!string.IsNullOrEmpty(cursor) && int.TryParse(cursor, out int parsedPage))
            {   
                page = parsedPage;
            }

            var coins = await _coinGeckoService.GetCoinsAsync(page, limit);
            
            // Crear respuesta para scroll infinito
            var infiniteResponse = new InfiniteScrollResponse<CoinGeckoDto>
            {
                Data = coins,
                Count = coins.Count,
                HasMore = coins.Count == limit, // Si hay exactamente 'limit' elementos, probablemente hay más
                NextCursor = coins.Count == limit ? (page + 1).ToString() : null
            };

            // IMPORTANTE: Este endpoint NO usa SignalR
            // Solo devuelve datos cuando el usuario hace scroll
            return Ok(infiniteResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCoinById(string id)
    {
        try
        {
            var coin = await _coinGeckoService.GetCoinDataAsync(id);
            return Ok(coin);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}