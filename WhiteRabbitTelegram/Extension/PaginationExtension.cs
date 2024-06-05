using Microsoft.EntityFrameworkCore;

namespace WhiteRabbitTelegram.Extension;

public class PaginationExtension<T>
{
    public List<T> Items { get; set; }
    public int Page { get; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasNextPage => Page * PageSize < TotalCount;
    public bool HasPreviousPage => Page > 1;

    public PaginationExtension(List<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public static async Task<PaginationExtension<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)
    {
        int totalCount = query.Count();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return new(items, page, pageSize, totalCount);
    }
}
