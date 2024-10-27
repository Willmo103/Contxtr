using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Contxtr.Core.Models
{
    public class DocumentMetadata
    {
        public string Path { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public long SizeInBytes { get; set; }
        public Dictionary<string, string> AdditionalMetadata { get; set; } = new();
    }
}