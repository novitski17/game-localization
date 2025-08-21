namespace GameLocalization.Core.DTO.Languages
{
    public class UpdateLanguageDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public bool? IsEnabled { get; set; }
    }
}
