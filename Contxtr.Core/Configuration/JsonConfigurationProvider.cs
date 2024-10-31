using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Contxtr.Core.Configuration
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
