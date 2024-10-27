using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contxtr.Core.Interfaces
{
    public interface IDocumentProcessor
    {
        Task ProcessAsync(string filePath, CancellationToken cancellationToken = default);
        Task<IEnumerable> ProcessDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default);
    }
}
