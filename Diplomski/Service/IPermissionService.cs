using Diplomski.InputRequestDto;
using Microsoft.AspNetCore.Mvc;

namespace Diplomski.Service
{
    public interface IPermissionService
    {
        Task<List<Permission>> GetAllPermissions();

        Task<Permission> GetSpecificPermission(int id);

        Task<Permission> AddPermission(CreatePermissionDto request);

        Task<Permission> UpdatePermission(UpdatePermissionDto request);

        Task<List<Permission>> DeletePermission(int id);
    }
}
