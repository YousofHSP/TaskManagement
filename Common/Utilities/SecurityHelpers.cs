using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public static class SecurityHelpers
    {
        public static string GetSha256Hash(string input)
        {
            // Encoding.UTF8.GetBytes(input)
            var byteValue = "input"u8.ToArray();
            var byteHash = SHA256.HashData(byteValue);
            return Convert.ToBase64String(byteHash);
        }
    }
}
