using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Diplomski.InputRequestDto;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using Diplomski.Helpers;
using Microsoft.IdentityModel.Tokens;

namespace Diplomski.Service
{
    public class PermissionService(DataContext dataContext, IHttpContextAccessor httpContext, PermissionChecker permissionChecker) : IPermissionService
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IHttpContextAccessor _httpContext = httpContext;
        private readonly PermissionChecker _permissionChecker = permissionChecker;

        public async Task<List<Permission>> GetAllPermissions()
        {
            var userTemp = await _permissionChecker.CheckPermission("GetAllPermission");

            return await _dataContext.Permissions.ToListAsync();
        }

        public async Task<Permission> GetSpecificPermission(int id)
        {
            var permission = await _dataContext.Permissions.FindAsync(id);

            if (permission == null)
                throw new Exception("Specified permission does not exist!");

            return permission;
        }

        public async Task<Permission> AddPermission(CreatePermissionDto request)
        {
            var userTemp = await _permissionChecker.CheckPermission("AddPermission");


            if(request.name.IsNullOrEmpty() || request.description.IsNullOrEmpty())
            {
                throw new Exception("Both fields must contain a value");
            }

            var permission = new Permission
            {
                name = request.name,
                description = request.description,
                PermissionSets = new List<PermissionSet>()
            };

            var permissionSet = await _dataContext.PermissionSets
                .Include(ps => ps.Permissions)
                .SingleOrDefaultAsync(ps => ps.Name == "Admin");

            if (permissionSet == null)
                throw new InvalidOperationException("Permission set 'Admin' not found.");

            _dataContext.Permissions.Add(permission);

            permissionSet.Permissions.Add(permission);

            await _dataContext.SaveChangesAsync();

            var newPermission = await _dataContext.Permissions
                .FirstOrDefaultAsync(p => p.name ==  request.name);

            return await GetSpecificPermission(newPermission.id);
        }


        public async Task<Permission> UpdatePermission(UpdatePermissionDto request)
        {
            var userTemp = await _permissionChecker.CheckPermission("UpdatePermission");

            Permission permission = await GetSpecificPermission(request.id);

            if (permission == null)
                throw new Exception("No permission found for id passed");

            if (!request.name.IsNullOrEmpty())
            {
                permission.name = request.name;
            }

            if (!request.description.IsNullOrEmpty())
            {
                permission.description = request.description;
            }

            await _dataContext.SaveChangesAsync();

            return await GetSpecificPermission(request.id);
        }

        public async Task<List<Permission>> DeletePermission(int id)
        {
            var userTemp = await _permissionChecker.CheckPermission("DeletePermission");

            var permission = await _dataContext.Permissions.FindAsync(id);

            if (permission == null)
                throw new Exception($"No permission found for id: {id}");

            var permissionSets = await _dataContext.PermissionSets
                .Include(ps => ps.Permissions)
                .Where(ps => ps.Permissions.Contains(permission))
                .ToListAsync();

            foreach (var permissionSet in permissionSets)
            {
                permissionSet.Permissions.Remove(permission);
            }

            _dataContext.Remove(permission);

            await _dataContext.SaveChangesAsync();

            return await GetAllPermissions();
        }
    }
}
