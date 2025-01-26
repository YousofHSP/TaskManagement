using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SiteSettings
    {
        public string ElmahPath { get; set; } = null!;
        public string Url { get; set; } = null!;
        public JwtSettings JwtSettings { get; set; } = null!;
        public IdentitySettings IdentitySettings { get; set; } = null!;
    }

    public class IdentitySettings
    {
        public bool PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool PasswordRequireNonAlphanumeric { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool RequireUniqueEmail { get; set; }
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; } = null!;
        public string EncryptKey { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int NotBeforeMinutes { get; set; }
        public int ExpirationMinutes { get; set; }
    }
}