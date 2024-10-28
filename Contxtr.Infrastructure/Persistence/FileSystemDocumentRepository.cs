using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Contxtr.Core.Interfaces;
using Contxtr.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Contxtr.Infrastructure.Persistence
{
    public class FileSystemDocumentRepository : IDocumentRepository
    {
        private readonly string _baseDirectory;
        private readonly ILogger<FileSystemDocumentRepository> _logger;
        private readonly JsonSerializerOptions _jsonOptions;

        public FileSystemDocumentRepository(
            IConfiguration configuration,
            ILogger<FileSystemDocumentRepository> logger)
        {
            _baseDirectory = configuration.GetSection("Storage")["BaseDirectory"]
                ?? Path.Combine(Environment.CurrentDirectory, "documents");
            _logger = logger;
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
    }
}