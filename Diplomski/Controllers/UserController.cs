using Diplomski.InputRequestDto;
using Diplomski.OutputRequestDto;
using Diplomski.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Diplomski.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService;

        [HttpGet("GetAllUsers")]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsers());
        }

        [HttpGet("GetUserById")]
        public async Task<ActionResult<List<User>>> GetSpecificUser(int id)
        {
            return Ok(await _userService.GetSpecificUser(id));
        }

        [HttpGet("VerifyEmail/{emailHash}")]

        public async Task<ActionResult<VerifyEmailResponseDto>> VerifyEmail(string emailHash)
        {
            return Ok(await _userService.VerifyEmail(emailHash));
        }

        [HttpGet("ResetPasswordRequest")]

        public async Task<ActionResult<ResetPasswordRequestResponseDto>> ResetPasswordRequest(string email)
        {
            return Ok(await _userService.ResetPasswordRequest(email));
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginUserDto request)
        {
            return Ok(await _userService.Login(request));
        }

        [HttpPost("VerifyLoginCode")]

        public async Task<ActionResult<VerifyLoginCodeResponseDto>> VerifyLoginCode(string code)
        {
            return Ok(await _userService.VerifyLoginCode(code));
        }

        [HttpPost("RegisterUser")]
        public async Task<ActionResult<RegisterUserResponseDto>> RegisterUser(CreateUserDto request)
        {
            return Ok(await _userService.RegisterUser(request));
        }

        [HttpPut("ModifyUser")]

        public async Task<ActionResult<User>> UpdateUser(UpdateUserDto request)
        {
            return Ok(await _userService.UpdateUser(request));   
        }

        [HttpPut("UpdatePassword")]
        public async Task<ActionResult<UpdatePasswordResponseDto>> UpdatePassword(UpdateUserPasswordDto request)
        {
            return Ok(await _userService.UpdatePassword(request));
        }

        [HttpPut("ResetPassword/{emailHash}")]

        public async Task<ActionResult<UpdatePasswordResponseDto>> ResetPassword(string emailHash, string newPassword)
        {
            return Ok(await _userService.ResetPassword(emailHash, newPassword));
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult<DeleteUserResponseDto>> DeleteUser(int id)
        {
            return Ok(await _userService.DeleteUser(id));
        }
    }
}
