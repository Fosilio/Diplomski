using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Diplomski
{
    public class Permission
    {
        public int id { get; set; }

        public string name { get; set; } = string.Empty;

        public string description { get; set; } = string.Empty;
        
        [AllowNull]
        [JsonIgnore]
        public List<PermissionSet> PermissionSets { get; set; }
    }
}
