using Contxtr.Core.Models;

namespace Contxtr.Core.Interfaces
{
    public interface ICodebaseProcessor
    {
        Task<CodebaseStructure> ProcessCodebaseAsync(
            string rootPath,
            IgnorePatterns ignorePatterns,
            CancellationToken cancellationToken = default);
    }
}