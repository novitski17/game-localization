namespace GameLocalization.Core.Domain.Entities
{
    public class Language
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool IsEnabled { get; set; } = false;
        public ICollection<Translation> Translations { get; set; } = new List<Translation>();
    }
}
