using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contxtr.Core.Configuration
{
    public class StorageConfig
    {
        public string BaseDirectory { get; set; } = string.Empty;
        public bool EnableVersioning { get; set; } = true;
        public int MaxVersions { get; set; } = 5;
    }
}
