namespace GameLocalization.Api.Models.Responses.Common
{
    public class PagedResponse<TItem>
    {
        public IReadOnlyList<TItem> Items { get; init; } = Array.Empty<TItem>();
        public int Page { get; init; }
        public int PageSize { get; init; }
        public long Total { get; init; }
    }
}
