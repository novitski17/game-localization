namespace GameLocalization.Core.DTO.Languages
{
    public class CreateLanguageDto
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool IsEnabled { get; set; } = true;
    }
}
