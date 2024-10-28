using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contxtr.Core.Models
{
    public class DirectoryNode
    {
        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public ICollection<DirectoryNode> Subdirectories { get; set; } = new List<DirectoryNode>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public string RelativePath { get; set; } = string.Empty;
    }
}
