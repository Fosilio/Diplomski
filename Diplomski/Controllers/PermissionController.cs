using Diplomski.InputRequestDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Diplomski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController(IPermissionService permissionService) : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        [HttpGet("AllPermissions"), Authorize]

        public async Task<ActionResult<List<Permission>>> GetAllPermissions()
        {
            return (await _permissionService.GetAllPermissions());
        }

        [HttpGet("GetPermissionById/{id}"), Authorize]

        public async Task<ActionResult<Permission>> GetSpecificPermission (int id)
        {
            return Ok(await _permissionService.GetSpecificPermission(id));
        }

        [HttpPost("AddNewPermission"), Authorize]

        public async Task<ActionResult<Permission>> AddPermission (CreatePermissionDto request)
        {
            return Ok(await _permissionService.AddPermission(request));
        }

        [HttpPut("UpdatePermission"), Authorize]
        public async Task<ActionResult<Permission>> UpdatePermission (UpdatePermissionDto request)
        {
            return Ok(await _permissionService.UpdatePermission(request));
        }

        [HttpDelete("DeletePermission"), Authorize]

        public async Task<ActionResult<List<Permission>>> DeletePermission (int id)
        {
            return Ok(await _permissionService.DeletePermission(id));
        }
    }
}
