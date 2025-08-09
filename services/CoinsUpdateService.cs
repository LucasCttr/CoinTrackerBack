using coninTracker.API.DTOs.CoinGecko;
using coninTracker.API.DTOs.Common;
using coninTracker.API.Hubs;
using coninTracker.API.Services;
using Microsoft.AspNetCore.SignalR;

namespace coninTracker.API.Services;

public class CoinsUpdateService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<CoinsHub> _hubContext;
    private readonly ILogger<CoinsUpdateService> _logger;

    public CoinsUpdateService(
        IServiceScopeFactory scopeFactory,
        IHubContext<CoinsHub> hubContext,
        ILogger<CoinsUpdateService> logger)
    {
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
        _logger = logger;
    }

    // Método que se ejecuta en segundo plano
    // Se encarga de enviar actualizaciones periódicas sobre las monedas
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var coinGeckoService = scope.ServiceProvider.GetRequiredService<ICoinGeckoService>();

                // Obtener datos actualizados
                var coins = await coinGeckoService.GetCoinsAsync(1, 20);

                var response = new InfiniteScrollResponse<CoinGeckoDto>
                {
                    Data = coins,
                    Count = coins.Count,
                    HasMore = coins.Count == 20,
                    NextCursor = coins.Count == 20 ? "2" : null
                };

                // Enviar a todos los clientes conectados
                await _hubContext.Clients.All.SendAsync("CoinsUpdated", response, stoppingToken);

                _logger.LogInformation($"Enviadas actualizaciones de {coins.Count} coins a los clientes");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar coins");
            }

            // Esperar 1 minuto
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
