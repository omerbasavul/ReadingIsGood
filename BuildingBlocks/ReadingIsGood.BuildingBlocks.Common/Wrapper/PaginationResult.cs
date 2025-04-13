namespace ReadingIsGood.BuildingBlocks.Common.Wrapper;

public record PaginationResult
{
    public int CurrentPage { get; set; } = 1;
    public uint TotalPageCount { get; set; } = 0;
    public long TotalResultCount { get; set; } = 0;

    public static PaginationResult Create(int pageNumber, long totalResultCount, int pageSize)
    {
        return new PaginationResult
        {
            CurrentPage = pageNumber,
            TotalResultCount = totalResultCount,
            TotalPageCount = (uint)Math.Ceiling((double)totalResultCount / pageSize)
        };
    }
}