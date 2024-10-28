using System.Text;
using Contxtr.Core.Interfaces;
using Contxtr.Core.Models;
using Microsoft.Extensions.Logging;

namespace Contxtr.Infrastructure.Output
{
    public class MarkdownDocumentWriter : IDocumentWriter
    {
        private readonly ILogger<MarkdownDocumentWriter> _logger;

        public MarkdownDocumentWriter(ILogger<MarkdownDocumentWriter> logger)
        {
            _logger = logger;
        }

        public async Task WriteAsync(Document document, string outputPath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Writing document to: {OutputPath}", outputPath);

            var markdown = new StringBuilder();

            // Add document metadata as YAML front matter
            markdown.AppendLine("---");
            markdown.AppendLine($"path: {document.Metadata.Path}");
            markdown.AppendLine($"language: {document.Metadata.Language}");
            markdown.AppendLine($"owner: {document.Metadata.Owner}");
            markdown.AppendLine($"size: {document.Metadata.SizeInBytes}");
            markdown.AppendLine($"created: {document.CreatedAt:yyyy-MM-dd HH:mm:ss}");
            markdown.AppendLine($"version: {document.Version}");
            markdown.AppendLine("---");
            markdown.AppendLine();
            markdown.AppendLine();

            // Add file path as header
            markdown.AppendLine($"# {document.Metadata.Path}");
            markdown.AppendLine();
            markdown.AppendLine();

            // Add content with language-specific code block
            markdown.AppendLine($"```{document.Metadata.Language}");
            markdown.AppendLine(document.Content.RawContent);
            markdown.AppendLine("```");

            // Ensure directory exists
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(outputPath, markdown.ToString(), cancellationToken);
            _logger.LogInformation("Document written successfully");
        }

        public async Task WriteAsync(IEnumerable<Document> documents, string outputPath, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Writing multiple documents to: {OutputPath}", outputPath);

            var markdown = new StringBuilder();

            foreach (var document in documents)
            {
                // Add file path as header
                markdown.AppendLine($"# {document.Metadata.Path}");
                markdown.AppendLine();

                // Add content with language-specific code block
                markdown.AppendLine($"```{document.Metadata.Language}");
                markdown.AppendLine(document.Content.RawContent);
                markdown.AppendLine("```");
                markdown.AppendLine();
            }

            // Ensure directory exists
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(outputPath, markdown.ToString(), cancellationToken);
            _logger.LogInformation("All documents written successfully");
        }
    }
}