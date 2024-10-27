using System.Collections.Generic;

namespace Contxtr.Core.Models
{

    public class DocumentContent
    {
        public string RawContent { get; set; } = string.Empty;
        public List<ContentLine> Lines { get; set; } = new();
        public string Hash { get; set; } = string.Empty;
    }

    public class ContentLine
    {
        public int Number { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}