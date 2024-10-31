using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Contxtr.Core.Configuration;
using Contxtr.Core.Interfaces;
using Contxtr.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Contxtr.Infrastructure.Persistence
{
    public class FileSystemDocumentRepository : IDocumentRepository
    {
        private readonly ILogger<FileSystemDocumentRepository> _logger;
        private readonly ContxtrConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseDirectory;

        public FileSystemDocumentRepository(
            ILogger<FileSystemDocumentRepository> logger,
            IContxtrConfigurationProvider configProvider)
        {
            _logger = logger;
            _configuration = configProvider.LoadConfigurationAsync().GetAwaiter().GetResult();
            _baseDirectory = _configuration.Storage.BaseDirectory;
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            Directory.CreateDirectory(_baseDirectory);
        }

        public async Task<Document> SaveAsync(Document document, CancellationToken cancellationToken = default)
        {
            var documentPath = GetDocumentPath(document.Id);

            if (_configuration.Storage.EnableVersioning && File.Exists(documentPath))
            {
                await CreateVersionAsync(documentPath, cancellationToken);
            }

            var json = JsonSerializer.Serialize(document, _jsonOptions);
            await File.WriteAllTextAsync(documentPath, json, cancellationToken);

            _logger.LogInformation("Document saved: {DocumentId}", document.Id);
            return document;
        }

        public async Task<Document?> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            var documentPath = GetDocumentPath(id);
            if (!File.Exists(documentPath))
            {
                return null;
            }

            var json = await File.ReadAllTextAsync(documentPath, cancellationToken);
            return JsonSerializer.Deserialize<Document>(json, _jsonOptions);
        }

        public async Task<IEnumerable<Document>> GetByPathAsync(string path, CancellationToken cancellationToken = default)
        {
            var documents = new List<Document>();
            var files = Directory.EnumerateFiles(_baseDirectory, "*.json", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file, cancellationToken);
                    var document = JsonSerializer.Deserialize<Document>(json, _jsonOptions);
                    if (document?.Metadata.Path.StartsWith(path, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        documents.Add(document);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading document file: {FilePath}", file);
                }
            }

            return documents;
        }

        private string GetDocumentPath(string id)
        {
            return Path.Combine(_baseDirectory, $"{id}.json");
        }

        private async Task CreateVersionAsync(string documentPath, CancellationToken cancellationToken)
        {
            var versionDir = Path.Combine(Path.GetDirectoryName(documentPath)!, "versions");
            Directory.CreateDirectory(versionDir);

            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var versionPath = Path.Combine(versionDir,
                $"{Path.GetFileNameWithoutExtension(documentPath)}_{timestamp}.json");

            // Replace CopyAsync with asynchronous copy using FileStream
            using (var sourceStream = new FileStream(documentPath, FileMode.Open, FileAccess.Read))
            using (var destinationStream = new FileStream(versionPath, FileMode.Create, FileAccess.Write))
            {
                await sourceStream.CopyToAsync(destinationStream, cancellationToken);
            }

            // Clean up old versions if needed
            if (_configuration.Storage.MaxVersions > 0)
            {
                var versions = Directory.GetFiles(versionDir)
                    .OrderByDescending(f => f)
                    .Skip(_configuration.Storage.MaxVersions);

                foreach (var oldVersion in versions)
                {
                    try
                    {
                        File.Delete(oldVersion);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deleting old version: {Path}", oldVersion);
                    }
                }
            }
        }
    }
}