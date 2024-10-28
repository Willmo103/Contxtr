using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Contxtr.Infrastructure.Services
{
    public class HashingService
    {
        public string ComputeHash(string content)
        {
            using var sha256 = SHA256.Create();
            var contentBytes = Encoding.UTF8.GetBytes(content);
            var hashBytes = sha256.ComputeHash(contentBytes);
            return Convert.ToHexString(hashBytes).ToLower();
        }
    }
}