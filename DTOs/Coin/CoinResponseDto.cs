// DTOs/CoinResponseDto.cs
public class CoinResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public decimal PriceChangePercentage24h { get; set; }
    public int MarketCapRank { get; set; }
}