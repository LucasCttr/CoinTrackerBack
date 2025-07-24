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
        int page = 1, 
        int pageSize = 20,
        string? sortBy = null,
        string? sortOrder = null)
    {
        try
        {
            var coins = await _coinGeckoService.GetCoinsAsync(page, pageSize);
            
            // 🎯 Crear respuesta paginada
            var paginatedResponse = new PaginatedResponse<CoinGeckoDto>
            {
                Data = coins,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = 100, // CoinGecko no devuelve total, usar estimado
                TotalPages = (int)Math.Ceiling(100.0 / pageSize)
            };

            return Ok(paginatedResponse);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCoin(string id)
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