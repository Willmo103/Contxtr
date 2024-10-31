using Contxtr.Core.Models;

namespace Contxtr.Core.Interfaces
{
    public interface IDocumentWriter
    {
        Task WriteAsync(Document document, string outputPath, CancellationToken cancellationToken = default);
        Task<IEnumerable<Document>> WriteAsync(IEnumerable<Document> documents, string outputPath, CancellationToken cancellationToken = default);
    }
}