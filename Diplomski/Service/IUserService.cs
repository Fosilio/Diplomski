using Diplomski.InputRequestDto;
using Diplomski.OutputRequestDto;
using Microsoft.AspNetCore.Mvc;

namespace Diplomski.Service
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsers();

        Task<User> GetSpecificUser(int id);

        Task<VerifyEmailResponseDto> VerifyEmail(string emailHash);

        Task<ResetPasswordRequestResponseDto> ResetPasswordRequest(string email);

        Task<LoginResponseDto> Login(LoginUserDto request);

        Task<VerifyLoginCodeResponseDto> VerifyLoginCode(string code);

        Task<RegisterUserResponseDto> RegisterUser(CreateUserDto request);

        Task<User> UpdateUser(UpdateUserDto request);

        Task<UpdatePasswordResponseDto> ResetPassword(string emailHash, string newPassword);

        Task<UpdatePasswordResponseDto> UpdatePassword(UpdateUserPasswordDto request);

        Task<DeleteUserResponseDto> DeleteUser(int id);
    }
}
