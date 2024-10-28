using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contxtr.Core.Interfaces;
using Contxtr.Infrastructure.Persistence;
using Contxtr.Infrastructure.Processing;
using Contxtr.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Contxtr.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddContxtrInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<HashingService>();
            services.AddScoped<IDocumentProcessor, DocumentProcessor>();
            services.AddScoped<IDocumentRepository, FileSystemDocumentRepository>();

            return services;
        }
    }
}