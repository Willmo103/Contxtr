using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contxtr.Core.Configuration
{
    public interface IContxtrConfigurationProvider
    {
        Task<ContxtrConfiguration> LoadConfigurationAsync(CancellationToken cancellationToken = default);
    }
}
