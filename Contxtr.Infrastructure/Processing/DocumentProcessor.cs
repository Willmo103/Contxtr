using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contxtr.Core.Interfaces;
using Contxtr.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Contxtr.Infrastructure.Services;


namespace Contxtr.Infrastructure.Processing
{
    public class DocumentProcessor : IDocumentProcessor
    {
        private readonly ILogger<DocumentProcessor> _logger;
        private readonly IReadOnlyDictionary<string, string> _languageMap;
        private readonly HashingService _hashingService;

        public DocumentProcessor(
            ILogger<DocumentProcessor> logger,
            IConfiguration configuration,
            HashingService hashingService)
        {
            _logger = logger;
            _hashingService = hashingService;
            var languageMap = new Dictionary<string, string>();
            configuration.GetSection("LanguageMap").Bind(languageMap);
            _languageMap = languageMap;
        }

        public async Task<Document> ProcessAsync(string filePath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Processing file: {FilePath}", filePath);

            var fileInfo = new FileInfo(filePath);
            var content = await File.ReadAllTextAsync(filePath, cancellationToken);

            return new Document
            {
                Metadata = new DocumentMetadata
                {
                    Path = filePath,
                    Language = GetLanguage(filePath),
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
            var files = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories)
                .Where(file => _languageMap.ContainsKey(Path.GetExtension(file).ToLower()));

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

            return documents;
        }


        private string GetLanguage(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            return _languageMap.TryGetValue(extension, out var language) ? language : "plaintext";
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
