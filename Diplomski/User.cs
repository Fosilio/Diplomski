using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Diplomski
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Byte[] PasswordHash { get; set; }
        public Byte[] PasswordSalt { get; set; }
       
        [AllowNull]
        public PermissionSet PermissionSet { get; set; }

        public Byte[] EmailHash { get; set; }

        public bool IsVerified { get; set; } = false;
        
    }
}
