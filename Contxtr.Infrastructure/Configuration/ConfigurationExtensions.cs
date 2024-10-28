using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contxtr.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Contxtr.Infrastructure.Configuration
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddContxtrConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Determine which provider to use based on configuration
            var provider = configuration.GetValue<string>("Contxtr:ConfigurationProvider")?.ToLower() ?? "json";

            services.AddSingleton<IContxtrConfigurationProvider>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<BaseConfigurationProvider>>();

                return provider switch
                {
                    "sql" => new SqlConfigurationProvider(
                        (ILogger<SqlConfigurationProvider>)logger,
                        configuration),
                    "json" or _ => new JsonConfigurationProvider(
                        (ILogger<JsonConfigurationProvider>)logger,
                        configuration)
                };
            });

            return services;
        }
    }
}
