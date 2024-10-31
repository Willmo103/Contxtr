using Contxtr.Core.Interfaces;
using Contxtr.Infrastructure.Output;
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
            services.AddScoped<IDocumentProcessor, DocumentProcessor>();
            services.AddScoped<ICodebaseProcessor, CodebaseProcessor>();
            services.AddScoped<IDocumentRepository, FileSystemDocumentRepository>();
            services.AddScoped<IDocumentWriter, MarkdownDocumentWriter>();
            services.AddSingleton<HashingService>();

            return services;
        }
    }
}