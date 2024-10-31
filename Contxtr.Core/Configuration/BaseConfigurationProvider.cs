using Contxtr.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Contxtr.Core.Configuration
{
    public abstract class BaseConfigurationProvider : IContxtrConfigurationProvider
    {
        protected readonly ILogger _logger;
        protected readonly IConfiguration _configuration;

        protected BaseConfigurationProvider(
            ILogger logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public abstract Task<ContxtrConfiguration> LoadConfigurationAsync(
            CancellationToken cancellationToken = default);

        protected virtual ContxtrConfiguration ApplyDefaults(ContxtrConfiguration config)
        {
            // Add default ignore patterns
            if (config.Ignore.EnableSystemIgnores)
            {
                config.Ignore.Patterns.AddRange(new[]
                {
                    new IgnorePattern { Pattern = "bin", Description = "Binary output directory" },
                    new IgnorePattern { Pattern = "obj", Description = "Intermediate output directory" },
                    new IgnorePattern { Pattern = "node_modules", Description = "Node.js dependencies" },
                    // Add more defaults...
                });
            }
            _logger.LogInformation("Loaded configuration: {@Config}", config);
            return config;
        }
    }
}
