using Contxtr.Core.Models;

namespace Contxtr.Core.Interfaces
{
    public interface IDocumentRepository
    {
        Task SaveAsync(Document document, CancellationToken cancellationToken = default);
        Task GetAsync(string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Document>> GetByPathAsync(string path, CancellationToken cancellationToken = default);
    }
}
