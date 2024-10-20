using Diplomski.Helpers;
using Diplomski.InputRequestDto;
using Diplomski.Migrations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using System.Linq;

namespace Diplomski.Service
{
    public class PermissionSetService(DataContext dataContext, PermissionChecker permissionChecker) : IPermissionSetService
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly PermissionChecker _permissionChecker = permissionChecker;
        public async Task<List<PermissionSet>> GetAllPermissionSets()
        {
            var userTemp = await _permissionChecker.CheckPermission("GetAllPermissionSets");

            return await _dataContext.PermissionSets.Include(ps => ps.Permissions).ToListAsync();
        }

        public async Task<PermissionSet> GetSpecificPermissionSet(int id)
        {
            var permissionSet = await _dataContext.PermissionSets
                .Include(ps => ps.Permissions)
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (permissionSet == null)
                throw new Exception($"No permission set found for id: {id}");

            return permissionSet;
        }

        public async Task<PermissionSet> AddPermissionSet(CreatePermissionSetDto request)
        {
            var userTemp = await _permissionChecker.CheckPermission("AddPermissionSet");

            if(request.Name.IsNullOrEmpty())
            {
                throw new Exception("No name specified for permissionSet!");
            }

            var permissionSet = new PermissionSet { 
                Name = request.Name
            };
            
            _dataContext.PermissionSets.Add(permissionSet);
            await _dataContext.SaveChangesAsync();

            return permissionSet;
        }

        public async Task<PermissionSet> UpdatePermissionSet(UpdatePermissionSetDto request)
        {
            var userTemp = await _permissionChecker.CheckPermission("UpdatePermissionSet");

            var permissionSet = await GetSpecificPermissionSet(request.Id);

            if (permissionSet == null)
               throw new Exception($"No PermissionSet found for id: {request.Id}");


            if (!string.IsNullOrEmpty(request.Name))
            {
                permissionSet.Name = request.Name;
            }

            if (!request.UsersId.IsNullOrEmpty())
            {

                foreach (var userId in request.UsersId)
                {
                    var user = permissionSet.Users.Find(user => user.Id == userId);

                    if(user == null)
                    {
                        user = await _dataContext.Users.Include(u => u.PermissionSet).FirstOrDefaultAsync(u => u.Id == userId);

                        user.PermissionSet = permissionSet;

                        permissionSet.Users.Add(user);

                    }
                    else
                    {
                        return null;
                    }
                }
            }

            if (request.PermissionsId != null)
            {

                foreach (var permissionId in request.PermissionsId)
                {
                    var permission = permissionSet.Permissions.Find(permission => permission.id == permissionId);

                    if (permission == null)
                    {
                        permission = await _dataContext.Permissions.FindAsync(permissionId);

                        permissionSet.Permissions.Add(permission);
                    } 
                    else
                    {
                        return null;
                    }
                }
            }

            await _dataContext.SaveChangesAsync();

            return await GetSpecificPermissionSet(request.Id);
        }


        public async Task<PermissionSet> DeletePermissionSet(int id)
        {
            var userTemp = await _permissionChecker.CheckPermission("DeletePermissionSet");

            var permissionSet = await GetSpecificPermissionSet(id);

            if (permissionSet == null)
                return null;

            _dataContext.PermissionSets.Remove(permissionSet);

            await _dataContext.SaveChangesAsync();

            return permissionSet;
        }

        public async Task<List<Permission>> GetAllPermissionsForPermissionSet(string permissionSetName)
        {
            var userTemp = await _permissionChecker.CheckPermission("GetAllPermissionsForPermissionSet");

            var permissionSet = await _dataContext.PermissionSets
                .Include(ps => ps.Permissions) 
                .FirstOrDefaultAsync(ps => ps.Name == permissionSetName);

            
            if (permissionSet == null)
                return null;

            // Return the list of permissions
            return permissionSet.Permissions.ToList();  
        }
    }
}
