namespace coninTracker.API.DTOs.Common;

public class InfiniteScrollResponse<T>
{
    public List<T> Data { get; set; } = new();
    public string? NextCursor { get; set; }
    public bool HasMore { get; set; }
    public int Count { get; set; }
}
