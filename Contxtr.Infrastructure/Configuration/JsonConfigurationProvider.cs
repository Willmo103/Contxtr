using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contxtr.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Contxtr.Infrastructure.Configuration
{
    public class JsonConfigurationProvider : BaseConfigurationProvider
    {
        public JsonConfigurationProvider(
            ILogger<JsonConfigurationProvider> logger,
            IConfiguration configuration)
            : base(logger, configuration)
        {
        }

        public override async Task<ContxtrConfiguration> LoadConfigurationAsync(
            CancellationToken cancellationToken = default)
        {
            var config = new ContxtrConfiguration();

            _configuration.GetSection("Contxtr").Bind(config);

            return ApplyDefaults(config);
        }
    }
}
