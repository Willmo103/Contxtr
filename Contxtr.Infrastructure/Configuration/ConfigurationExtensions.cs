using Contxtr.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Contxtr.Infrastructure.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContxtrConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register configuration itself
            var configProvider = new JsonConfigurationProvider(
                services.BuildServiceProvider().GetRequiredService<ILogger<JsonConfigurationProvider>>(),
                configuration);

            var contxtrConfig = configProvider.LoadConfigurationAsync().GetAwaiter().GetResult();
            services.AddSingleton(contxtrConfig);

            // Register configuration provider
            services.AddSingleton<IContxtrConfigurationProvider>(sp =>
                new JsonConfigurationProvider(
                    sp.GetRequiredService<ILogger<JsonConfigurationProvider>>(),
                    configuration));

            return services;
        }
    }
}