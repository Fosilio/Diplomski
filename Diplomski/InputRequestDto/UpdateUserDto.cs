using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Diplomski.InputRequestDto
{
    public class UpdateUserDto
    {
        public int Id { get; set; }
        [AllowNull]
        public string Name { get; set; } = string.Empty;
        [AllowNull]
        public string LastName { get; set; } = string.Empty;
        [AllowNull]
        public string UserName { get; set; } = string.Empty;
        [AllowNull]
        public string Email { get; set; } = string.Empty;
        [AllowNull]
        [JsonIgnore]
        public PermissionSet? PermissionSet { get; set; }
        [AllowNull]
        public int permissionSetId { get; set; }
    }
}
