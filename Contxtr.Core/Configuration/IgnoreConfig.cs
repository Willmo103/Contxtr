using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contxtr.Core.Models;

namespace Contxtr.Core.Configuration
{
    public class IgnoreConfig
    {
        public List<IgnorePattern> Patterns { get; set; } = new();
        public bool EnableGitIgnore { get; set; } = true;
        public bool EnableSystemIgnores { get; set; } = true;
    }
}
