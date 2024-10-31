using System.CommandLine;
using Contxtr.Core.Configuration;
using Contxtr.Core.Interfaces;
using Contxtr.Core.Models;
using Contxtr.Infrastructure.Configuration;
using Contxtr.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Spectre.Console;

namespace Contxtr.CLI
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            // Setup configuration and services
            var services = ConfigureServices();

            // Create root command
            var rootCommand = new RootCommand("Contxtr - Context-aware document processing toolkit");

            // Add commands
            rootCommand.AddCommand(CreateProcessCommand(services));
            rootCommand.AddCommand(CreateFlattenCommand(services));
            rootCommand.AddCommand(CreateConfigCommand(services));

            // Execute command
            return await rootCommand.InvokeAsync(args);
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Configure logging
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/contxtr-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            services.AddLogging(builder => builder.AddSerilog(Log.Logger));

            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables("CONTXTR_")
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Add Contxtr services
            services.AddContxtrConfiguration(configuration);
            services.AddContxtrInfrastructure(configuration);

            return services.BuildServiceProvider();
        }

        private static Command CreateProcessCommand(ServiceProvider services)
        {
            var command = new Command("process", "Process documents and extract content");

            var inputOption = new Option<string>(
                new[] { "--input", "-i" },
                "Input file or directory path")
            { IsRequired = true };

            var outputOption = new Option<string>(
                new[] { "--output", "-o" },
                "Output directory path");

            var recursiveOption = new Option<bool>(
                new[] { "--recursive", "-r" },
                () => false,
                "Process directories recursively");

            command.AddOption(inputOption);
            command.AddOption(outputOption);
            command.AddOption(recursiveOption);

            command.SetHandler(async (string input, string? output, bool recursive) =>
            {
                await using var scope = services.CreateAsyncScope();
                var processor = scope.ServiceProvider.GetRequiredService<IDocumentProcessor>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                try
                {
                    AnsiConsole.Status()
                        .Start("Processing documents...", async ctx =>
                        {
                            if (File.Exists(input))
                            {
                                var document = await processor.ProcessAsync(input);
                                if (output != null)
                                {
                                    var writer = scope.ServiceProvider.GetRequiredService<IDocumentWriter>();
                                    await writer.WriteAsync(document, output);
                                }
                            }
                            else if (Directory.Exists(input))
                            {
                                var documents = await processor.ProcessDirectoryAsync(input);
                                if (output != null)
                                {
                                    var writer = scope.ServiceProvider.GetRequiredService<IDocumentWriter>();
                                    await writer.WriteAsync(documents, output);
                                }
                            }
                            else
                            {
                                throw new FileNotFoundException("Input path not found", input);
                            }
                        });

                    AnsiConsole.MarkupLine("[green]Processing completed successfully![/]");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error processing documents");
                    AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                }
            }, inputOption, outputOption, recursiveOption);

            return command;
        }

        private static Command CreateFlattenCommand(ServiceProvider services)
        {
            var command = new Command("flatten", "Flatten a codebase into a single document");

            var inputOption = new Option<string>(
                new[] { "--input", "-i" },
                "Input directory path")
            { IsRequired = true };

            var outputOption = new Option<string>(
                new[] { "--output", "-o" },
                "Output file path");

            var formatOption = new Option<string>(
                new[] { "--format", "-f" },
                () => "markdown",
                "Output format (markdown, json)");

            command.AddOption(inputOption);
            command.AddOption(outputOption);
            command.AddOption(formatOption);

            command.SetHandler(async (string input, string? output, string format) =>
            {
                await using var scope = services.CreateAsyncScope();
                var processor = scope.ServiceProvider.GetRequiredService<ICodebaseProcessor>();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                try
                {
                    AnsiConsole.Status()
                        .Start("Flattening codebase...", async ctx =>
                        {
                            // Create ignore patterns
                            var ignorePatterns = new IgnorePatterns
                            {
                                Source = "default",
                                Patterns = new List<IgnorePattern>()
                            };

                            var result = await processor.ProcessCodebaseAsync(
                                input,
                                ignorePatterns,
                                CancellationToken.None);

                            if (output != null)
                            {
                                var writer = scope.ServiceProvider.GetRequiredService<IDocumentWriter>();
                                await writer.WriteAsync(result.Documents, output);
                            }
                        });

                    AnsiConsole.MarkupLine("[green]Flattening completed successfully![/]");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error flattening codebase");
                    AnsiConsole.MarkupLine($"[red]Error: {ex.Message}[/]");
                }
            }, inputOption, outputOption, formatOption);

            return command;
        }


        private static Command CreateConfigCommand(ServiceProvider services)
        {
            var command = new Command("config", "Manage configuration");

            var listCommand = new Command("list", "List current configuration");
            listCommand.SetHandler(() =>
            {
                var config = services.GetRequiredService<ContxtrConfiguration>();

                var table = new Table()
                    .AddColumn("Setting")
                    .AddColumn("Value");

                table.AddRow("Base Directory", config.Storage.BaseDirectory);
                table.AddRow("Enable Versioning", config.Storage.EnableVersioning.ToString());
                table.AddRow("Max Versions", config.Storage.MaxVersions.ToString());
                table.AddRow("Parallel Processing", config.Processing.ParallelProcessing.ToString());
                // Add more configuration values as needed

                AnsiConsole.Write(table);
            });

            command.AddCommand(listCommand);

            return command;
        }
    }
}