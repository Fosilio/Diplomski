using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Diplomski.InputRequestDto
{
    public class UpdatePermissionSetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [AllowNull]
        [JsonIgnore]
        public List<User>? Users { get; set; }
        [AllowNull]
        [JsonIgnore]
        public List<Permission>? Permissions { get; set; }

        public List<int>? UsersId {  get; set; }

        public List<int>? PermissionsId { get; set; }
    }
}
