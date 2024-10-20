using Diplomski.InputRequestDto;
using Microsoft.AspNetCore.Mvc;

namespace Diplomski.Service
{
    public interface IPermissionSetService
    {
        Task<List<PermissionSet>> GetAllPermissionSets();

        Task<PermissionSet> GetSpecificPermissionSet(int id);

        Task<PermissionSet> AddPermissionSet(CreatePermissionSetDto request);

        Task<PermissionSet> UpdatePermissionSet(UpdatePermissionSetDto request);

        Task<PermissionSet> DeletePermissionSet(int id);

        Task<List<Permission>> GetAllPermissionsForPermissionSet(string permissionSetName);
    }
}
