namespace GameLocalization.Core.DTO.Common
{
    public class PagedSlice<T>
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
        public long Total { get; init; }
    }
}
