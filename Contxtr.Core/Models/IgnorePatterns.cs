namespace Contxtr.Core.Models
{
    public class IgnorePatterns
    {
        public string Source { get; set; } = string.Empty;
        public ICollection<IgnorePattern> Patterns { get; set; } = new List<IgnorePattern>();
    }
}
