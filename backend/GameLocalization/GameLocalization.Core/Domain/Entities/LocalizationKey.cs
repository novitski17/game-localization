namespace GameLocalization.Core.Domain.Entities
{
    public class LocalizationKey
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = default!;

        public ICollection<Translation> Translations { get; set; } = new List<Translation>();
    }
}
 