using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Diplomski
{
    public class PermissionSet
    {
     

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        [AllowNull]
        public List<User> Users { get; set; }

        [AllowNull]
        public List<Permission> Permissions { get; set; }
    }
}
