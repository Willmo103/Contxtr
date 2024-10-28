using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contxtr.Core.Models;

namespace Contxtr.Core.Interfaces
{
    public interface IIgnorePatternProvider
    {
        Task<IgnorePatterns> LoadPatternsAsync(CancellationToken cancellationToken = default);
    }
}
