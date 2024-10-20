using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Security.Authentication;

namespace Diplomski.Helpers
{
    public class PermissionChecker(DataContext dataContext, IHttpContextAccessor httpContext)
    {
        private readonly DataContext dataContext = dataContext;
        private readonly IHttpContextAccessor httpContext = httpContext;
        public async Task<User> CheckPermission(string permission) {
            if (httpContext.HttpContext == null)
            {
                throw new Exception("You must be logged in to add an accommodation!");
            }

            var username = httpContext.HttpContext.Items["NameIdentifier"]?.ToString();

            var user = await this.dataContext.Users
                .Include(u => u.PermissionSet)
                .ThenInclude(ps => ps.Permissions)
                .FirstOrDefaultAsync(u => u.UserName == username) ?? throw new AuthenticationException();

            if (user.PermissionSet.Permissions.FirstOrDefault(p => p.name == permission) == null)
            {
                throw new UnauthorizedAccessException(permission);
            }

            return user;
        }
    }
}
