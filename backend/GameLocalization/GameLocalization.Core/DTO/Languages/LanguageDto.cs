namespace GameLocalization.Core.DTO.Languages
{
    public class LanguageDto
    {
        public Guid Id { get; init; }
        public string Code { get; init; } = default!;
        public string Name { get; init; } = default!;
        public bool IsEnabled { get; init; }
    }
}
