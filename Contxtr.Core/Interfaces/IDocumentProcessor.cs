using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contxtr.Core.Models;

namespace Contxtr.Core.Interfaces
{
    public interface IDocumentProcessor
    {
        Task<Document> ProcessAsync(string filePath, CancellationToken cancellationToken = default);
        Task<IEnumerable<Document>> ProcessDirectoryAsync(string directoryPath, CancellationToken cancellationToken = default);
    }
}

