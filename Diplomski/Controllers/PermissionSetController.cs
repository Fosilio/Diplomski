using Diplomski.InputRequestDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Diplomski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionSetController(IPermissionSetService permissionSetService) : ControllerBase
    {
        private readonly IPermissionSetService _permissionSetService;

        [HttpGet("AllPermissionSets"), Authorize]
        public async Task<ActionResult<List<PermissionSet>>> GetAllPermissionSets()
        {
            return Ok(await _permissionSetService.GetAllPermissionSets());
        }

        [HttpGet("GetPermissionSetById")]
        public async Task<ActionResult<PermissionSet>> GetSpecificPermissionSet(int id)
        {
            return Ok(await _permissionSetService.GetSpecificPermissionSet(id));
        }

        [HttpPost("AddNewPermissionSet"), Authorize]
        public async Task<ActionResult<List<PermissionSet>>> AddPermissionSet(CreatePermissionSetDto request)
        {
            return Ok(await _permissionSetService.AddPermissionSet(request));
        }

        [HttpPut("UpdatePermissionSet"), Authorize]
        public async Task<ActionResult<PermissionSet>> UpdatePermissionSet(UpdatePermissionSetDto request)
        {
            return Ok(await _permissionSetService.UpdatePermissionSet(request));
        }


        [HttpDelete("DeletePermissionSet"), Authorize]
        public async Task<ActionResult<List<PermissionSet>>> DeletePermissionSet(int id)
        {
            return Ok(await _permissionSetService.DeletePermissionSet(id));
        }

        [HttpGet("GetAllPermissionsForPermissionSet"), Authorize]

        public async Task<ActionResult<List<Permission>>> GetAllPermissionsForPermissionSet(string permissionSetName)
        {
            return await _permissionSetService.GetAllPermissionsForPermissionSet(permissionSetName);
        }
    }
}