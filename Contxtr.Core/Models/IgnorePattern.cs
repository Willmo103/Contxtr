using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contxtr.Core.Models
{
    public class IgnorePattern
    {
        public string Pattern { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsRegex { get; set; }
        public string? Category { get; set; }
    }
}
