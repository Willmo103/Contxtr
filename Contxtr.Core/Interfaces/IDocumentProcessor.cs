using Contxtr.Core.Models;

namespace Contxtr.Core.Interfaces
{
    public interface IDocumentProcessor
    {
        Task<Document> ProcessAsync(string filePath, CancellationToken cancellationToken = default);
        Task<IEnumerable<Document>> ProcessDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default);
    }
}

