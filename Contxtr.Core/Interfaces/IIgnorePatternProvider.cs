using Contxtr.Core.Models;

namespace Contxtr.Core.Interfaces
{
    public interface IIgnorePatternProvider
    {
        Task<IgnorePatterns> LoadPatternsAsync(CancellationToken cancellationToken = default);
    }
}
