namespace Contxtr.Core.Models
{
    public class IgnorePattern
    {
        public string Pattern { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsRegex { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Category { get; set; }
    }
}
