# Contxtr Package Guide

## Core Packages

### Microsoft.Extensions.* (v8.0.0)
Purpose: Provide core dependency injection, configuration, and logging abstractions.
```csharp
// Configuration.Abstractions - Define configuration interfaces
public interface IMyService
{
    string GetSetting();
}

// DependencyInjection.Abstractions - Register services
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyService(this IServiceCollection services)
    {
        services.AddScoped();
        return services;
    }
}

// Logging.Abstractions - Logging interface
public class MyService
{
    private readonly ILogger _logger;
    
    public MyService(ILogger logger)
    {
        _logger = logger;
        _logger.LogInformation("MyService initialized");
    }
}
```

### System.Linq.Async (v6.0.1)
Purpose: Provides async enumerable support for LINQ operations
```csharp
public class DocumentProcessor
{
    public async IAsyncEnumerable ProcessDocumentsAsync()
    {
        var files = Directory.EnumerateFiles("path");
        await foreach (var doc in files.ToAsyncEnumerable()
            .SelectAwait(async file => await ProcessFileAsync(file))
            .WhereAwait(async doc => await IsValidAsync(doc)))
        {
            yield return doc;
        }
    }
}
```

### OneOf (v3.0.263)
Purpose: Provides discriminated unions for better error handling
```csharp
public class ProcessingResult : OneOfBase
{
    public class ProcessingError
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}

public async Task<OneOf> ProcessAsync()
{
    try
    {
        var doc = await ProcessDocument();
        return doc;
    }
    catch (Exception ex)
    {
        return new ProcessingError { Message = "Processing failed", Exception = ex };
    }
}
```

## Infrastructure Packages

### Serilog (v3.1.1)
Purpose: Advanced logging capabilities
```csharp
public static class LoggerConfig
{
    public static void Configure()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(
                theme: AnsiConsoleTheme.Code,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("logs/contxtr-.log", 
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();
    }
}
```

### LibGit2Sharp (v0.27.2)
Purpose: Git repository handling
```csharp
public class GitProcessor
{
    public async Task ProcessRepository(string url)
    {
        // Clone repository
        Repository.Clone(url, "path/to/clone");
        
        using var repo = new Repository("path/to/clone");
        // Get latest commit
        var latest = repo.Head.Tip;
        
        // Get file changes
        var changes = repo.Diff.Compare();
        foreach (var change in changes)
        {
            // Process changed files
        }
    }
}
```

### Polly (v8.2.0)
Purpose: Resilience and transient fault handling
```csharp
public class ResiliencePolicy
{
    private static readonly IAsyncPolicy RetryPolicy =
        Policy
            .Handle()
            .Or()
            .WaitAndRetryAsync(3, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public async Task ExecuteWithRetry(
        Func<Task> action)
    {
        return await RetryPolicy.ExecuteAsync(action);
    }
}
```

## CLI Packages

### System.CommandLine (v2.0.0-beta4.22272.1)
Purpose: Command-line parsing and execution
```csharp
public class Program
{
    static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand("Contxtr CLI");
        
        var inputOption = new Option(
            aliases: new[] {"--input", "-i"},
            description: "Input directory path");
            
        var processCommand = new Command("process", "Process documents")
        {
            inputOption
        };
        
        processCommand.SetHandler(async (string input) =>
        {
            await ProcessDocuments(input);
        }, inputOption);
        
        rootCommand.AddCommand(processCommand);
        
        return await rootCommand.InvokeAsync(args);
    }
}
```

### Spectre.Console (v0.47.0)
Purpose: Better console UI and formatting
```csharp
public class ConsoleUI
{
    public async Task ShowProgress(IEnumerable files)
    {
        AnsiConsole.Write(new Rule("[yellow]Processing Files[/]"));
        
        await AnsiConsole.Progress()
            .AutoClear(false)
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn()
            })
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask("[green]Processing[/]");
                foreach (var file in files)
                {
                    task.Increment(1);
                    await Task.Delay(100);
                }
            });
    }

    public void ShowTree(Document doc)
    {
        var root = new Tree("Document Structure");
        var metadata = root.AddNode("[blue]Metadata[/]");
        metadata.AddNode($"Path: {doc.Metadata.Path}");
        metadata.AddNode($"Size: {doc.Metadata.SizeInBytes} bytes");
        
        AnsiConsole.Write(root);
    }
}
```

## Version Compatibility Notes

1. All Microsoft.Extensions.* packages should match .NET version (8.0.0)
2. Serilog and its sinks should be kept at latest stable versions
3. System.CommandLine is still in beta - consider version lock
4. LibGit2Sharp sometimes has compatibility issues - test thoroughly
