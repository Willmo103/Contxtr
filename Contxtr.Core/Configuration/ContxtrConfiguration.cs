using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contxtr.Core.Configuration
{
    public class ContxtrConfiguration
    {
        public LanguageMapConfig LanguageMap { get; set; } = new();
        public IgnoreConfig Ignore { get; set; } = new();
        public StorageConfig Storage { get; set; } = new();
        public ProcessingConfig Processing { get; set; } = new();
    }
}
