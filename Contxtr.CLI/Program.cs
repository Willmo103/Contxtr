// Program.cs (CLI)
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Contxtr.CLI
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var services = await ConfigureServices();
            var rootCommand = new RootCommand("Contxtr - Document Processing Tool");

            rootCommand.AddCommand(new FlattenCommand(services).Command);

            return await rootCommand.InvokeAsync(args);
        }

        private static async Task<IServiceProvider> ConfigureServices()
        {
            var services = new ServiceCollection();

            // Add configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables("CONTXTR_")
                .Build();

            // Add core services
            services.AddContxtrConfiguration(configuration);
            services.AddContxtrInfrastructure();

            // Add logging
            services.AddLogging(builder =>
            {
                builder.AddSerilog(new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger());
            });

            return services.BuildServiceProvider();
        }
    }
}