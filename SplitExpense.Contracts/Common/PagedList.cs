namespace SplitExpense.Contracts.Common;

public sealed class PagedList<T>
{
    public PagedList(IEnumerable<T> items, int page, int pageSize, int totalCount)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        Items = items.ToList();
    }

    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public IReadOnlyCollection<T> Items { get; }
}
