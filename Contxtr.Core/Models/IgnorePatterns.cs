using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contxtr.Core.Models
{
    public class IgnorePatterns
    {
        public string Source { get; set; } = string.Empty;
        public ICollection<IgnorePattern> Patterns { get; set; } = new List<IgnorePattern>();
    }
}
