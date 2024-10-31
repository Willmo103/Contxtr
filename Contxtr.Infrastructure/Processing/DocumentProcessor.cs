using Contxtr.Core.Configuration;
using Contxtr.Core.Interfaces;
using Contxtr.Core.Models;
using Contxtr.Infrastructure.Services;
using Microsoft.Extensions.Logging;


namespace Contxtr.Infrastructure.Processing
{
    public class DocumentProcessor : IDocumentProcessor
    {
        private readonly ILogger<DocumentProcessor> _logger;
        private readonly ContxtrConfiguration _configuration;
        private readonly HashingService _hashingService;

        public DocumentProcessor(
            ILogger<DocumentProcessor> logger,
            IContxtrConfigurationProvider configProvider,
            HashingService hashingService)
        {
            _logger = logger;
            _hashingService = hashingService;
            _configuration = configProvider.LoadConfigurationAsync().GetAwaiter().GetResult();
        }

        public async Task<Document> ProcessAsync(string filePath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Processing file: {FilePath}", filePath);

            var fileInfo = new FileInfo(filePath);
            var content = await File.ReadAllTextAsync(filePath, cancellationToken);
            var extension = Path.GetExtension(filePath).ToLower();

            return new Document
            {
                Metadata = new DocumentMetadata
                {
                    Path = filePath,
                    Language = _configuration.LanguageMap.Extensions.GetValueOrDefault(extension, "plaintext"),
                    Owner = GetFileOwner(fileInfo),
                    SizeInBytes = fileInfo.Length
                },
                Content = new DocumentContent
                {
                    RawContent = content,
                    Lines = content.Split(Environment.NewLine)
                        .Select((line, index) => new ContentLine
                        {
                            Number = index + 1,
                            Content = line
                        })
                        .ToList(),
                    Hash = _hashingService.ComputeHash(content)
                }
            };
        }

        public async Task<IEnumerable<Document>> ProcessDirectoryAsync(
            string directoryPath,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Processing directory: {DirectoryPath}", directoryPath);

            var documents = new List<Document>();
            var allowedExtensions = _configuration.LanguageMap.Extensions.Keys;

            var files = Directory.EnumerateFiles(directoryPath, "*.*",
                _configuration.Processing.FollowSymlinks
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly)
                .Where(file => allowedExtensions.Contains(Path.GetExtension(file).ToLower()))
                .Where(file => !ShouldIgnore(file));

            if (_configuration.Processing.ParallelProcessing)
            {
                var options = new ParallelOptions
                {
                    MaxDegreeOfParallelism = _configuration.Processing.MaxDegreeOfParallelism,
                    CancellationToken = cancellationToken
                };

                await Parallel.ForEachAsync(files, options, async (file, ct) =>
                {
                    try
                    {
                        var document = await ProcessAsync(file, ct);
                        lock (documents)
                        {
                            documents.Add(document);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing file: {FilePath}", file);
                    }
                });
            }
            else
            {
                foreach (var file in files)
                {
                    try
                    {
                        var document = await ProcessAsync(file, cancellationToken);
                        documents.Add(document);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing file: {FilePath}", file);
                    }
                }
            }

            return documents;
        }

        private bool ShouldIgnore(string path)
        {
            foreach (var pattern in _configuration.Ignore.Patterns)
            {
                if (path.Contains(pattern.Pattern, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private string GetFileOwner(FileInfo fileInfo)
        {
            try
            {
                var security = fileInfo.GetAccessControl();
                var owner = security.GetOwner(typeof(System.Security.Principal.NTAccount));
                return owner?.ToString() ?? "unknown";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Unable to get file owner for: {FilePath}", fileInfo.FullName);
                return "unknown";
            }
        }
    }
}