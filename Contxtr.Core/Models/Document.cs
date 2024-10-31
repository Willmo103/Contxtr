namespace Contxtr.Core.Models
{

    public class Document
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DocumentMetadata Metadata { get; set; } = new();
        public DocumentContent Content { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Version { get; set; } = "1.0.0";
    }
}