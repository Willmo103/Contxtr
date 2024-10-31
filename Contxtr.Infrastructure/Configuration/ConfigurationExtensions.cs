using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contxtr.Core.Configuration;
using Contxtr.Core.Interfaces;
using Contxtr.Infrastructure.Persistence;
using Contxtr.Infrastructure.Processing;
using Contxtr.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Contxtr.Infrastructure.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContxtrInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Load configuration once at startup
            var configProvider = new JsonConfigurationProvider(
                services.BuildServiceProvider().GetRequiredService<ILogger<JsonConfigurationProvider>>(),
                configuration);
            var contxtrConfig = configProvider.LoadConfigurationAsync().GetAwaiter().GetResult();

            services.AddSingleton(contxtrConfig);
            services.AddSingleton<HashingService>();
            services.AddScoped<IDocumentProcessor, DocumentProcessor>();
            services.AddScoped<IDocumentRepository, FileSystemDocumentRepository>();

            return services;
        }
    }
}
