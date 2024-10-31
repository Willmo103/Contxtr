namespace Contxtr.Core.Models
{
    public class CodebaseStructure
    {
        public string Name { get; set; } = string.Empty;
        public string RootPath { get; set; } = string.Empty;
        public DirectoryNode RootDirectory { get; set; } = new();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
    }
}
