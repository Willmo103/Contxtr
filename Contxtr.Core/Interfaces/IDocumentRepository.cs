using Contxtr.Core.Models;

namespace Contxtr.Core.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Document> SaveAsync(Document document, CancellationToken cancellationToken = default);
        Task<Document?> GetAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Document>> GetByPathAsync(string path, CancellationToken cancellationToken = default);
    }
}
