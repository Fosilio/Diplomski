using Diplomski.Helpers;
using Diplomski.InputRequestDto;
using Diplomski.OutputRequestDto;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using Azure;

namespace Diplomski.Service
{
    public class UserService(
           DataContext dataContext, 
           JwtSecurityTokenHandlerWrapper jwtSecurityTokenHandlerWrapper, 
           EmailVerificationBody emailBody, 
           VerifyLoginCodeVerification verifyLoginCodeVerification,
           EmailCodeVerificationBody emailCodeVerificationBody,
           EmailResetPasswordBody emailResetPasswordBody,
           ResetPasswordVerification resetPasswordVerification) : IUserService
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly JwtSecurityTokenHandlerWrapper _jwtSecurityTokenHandlerWrapper = jwtSecurityTokenHandlerWrapper;
        private readonly EmailVerificationBody _emailBody = emailBody;
        private readonly VerifyLoginCodeVerification _verifyLoginCodeVerification = verifyLoginCodeVerification;
        private readonly EmailCodeVerificationBody _emailCodeVerificationBody = emailCodeVerificationBody;
        private readonly EmailResetPasswordBody _emailResetPasswordBody = emailResetPasswordBody;
        private readonly ResetPasswordVerification _resetPasswordVerification = resetPasswordVerification;

        private const string emailSalt = "f3AP1hNfMud0Tl78nyIj";

        public async Task<List<User>> GetAllUsers()
        {
            return await _dataContext.Users
                 .Include(u => u.PermissionSet)
                 .ThenInclude(ps => ps.Permissions)
                 .ToListAsync();
        }

        public async Task<User> GetSpecificUser(int id)
        {
            var user = await _dataContext.Users
                .Include(u => u.PermissionSet)
                .ThenInclude(ps => ps.Permissions)
                .FirstOrDefaultAsync(u => u.Id == id);

            if(user == null)
            {
                throw new Exception($"User with id:{id} does not exist!");
            }

            return user;
        }

        public async Task<VerifyEmailResponseDto> VerifyEmail(string emailHash)
        {

            string decodedEmailHash = HttpUtility.UrlDecode(emailHash);

            decodedEmailHash = decodedEmailHash.Replace(" ", "+");

            byte[] inputEmailHash = Convert.FromBase64String(decodedEmailHash);

            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.EmailHash == inputEmailHash);

            if (user == null)
            {
                throw new Exception("User does not exist!");
            }

            user.IsVerified = true;

            await _dataContext.SaveChangesAsync();

            return new VerifyEmailResponseDto() 
            {
                Response = "User verified!"
            };
        }

        public async Task<ResetPasswordRequestResponseDto> ResetPasswordRequest(string email)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                throw new Exception("User not found!");
            }

            if(!user.IsVerified)
            {
                throw new Exception("User not verified");
            }

            byte[] emailHash = HashEmail(email);

            string emailHashString = Convert.ToBase64String(emailHash);

            string emailText = _emailResetPasswordBody.createBody(emailHashString);

            var sendEmail = new MimeMessage();
            sendEmail.From.Add(MailboxAddress.Parse(user.Email));
            sendEmail.To.Add(MailboxAddress.Parse(user.Email));
            sendEmail.Subject = "Verify Email Address";
            sendEmail.Body = new TextPart("html")
            {
                Text = emailText
            };

            var client = new SmtpClient();

            client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            client.Authenticate("jamakovicemrah6@gmail.com", /*ovdje ide password*/);
            client.Send(sendEmail);
            client.Disconnect(true);

            _resetPasswordVerification.StoreEmailHash(user.UserName, emailHash, TimeSpan.FromMinutes(5));

            return new ResetPasswordRequestResponseDto() 
            {
                Response = "Reset password request sent to e-mail!"
            };
        }

        public async Task<LoginResponseDto> Login(LoginUserDto request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Invalid username or password");
            }

            var user = await _dataContext.Users.Include(u => u.PermissionSet).FirstOrDefaultAsync(u => u.UserName == request.UserName);

            if (user == null)
            {
                throw new ArgumentException("Invalid username or password");
            }
            else
            {
                if(user.IsVerified == false)
                {
                    throw new Exception("Invalid username or password");
                }

                bool verification = VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt);

                if (verification)
                {

                    var verificationCode = GenerateVerificationCode();

                    _verifyLoginCodeVerification.StoreCode(user.UserName, verificationCode, TimeSpan.FromMinutes(5));

                    string emailText = _emailCodeVerificationBody.CreateBody(verificationCode);

                    var sendEmail = new MimeMessage();
                    sendEmail.From.Add(MailboxAddress.Parse(user.Email));
                    sendEmail.To.Add(MailboxAddress.Parse(user.Email));
                    sendEmail.Subject = "Verify Email Address";
                    sendEmail.Body = new TextPart("html")
                    {
                        Text = emailText
                    };

                    var client = new SmtpClient();

                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                    client.Authenticate("jamakovicemrah6@gmail.com", /*ovdje ide password*/);
                    client.Send(sendEmail);
                    client.Disconnect(true);

                    return new LoginResponseDto()
                    {
                        Response = "We have sent you a login code to your e-mail!"
                    };
                }
                else
                {
                    throw new ArgumentException("Invalid username or password");
                }
            }
        }

        public async Task<VerifyLoginCodeResponseDto> VerifyLoginCode(string code)
        {
            var username = _verifyLoginCodeVerification.GetUsernameByCode(code);

            if(username.IsNullOrEmpty())
            {
                throw new Exception($"Invalid username: {username}");
            }

            var token = await _jwtSecurityTokenHandlerWrapper.GenerateJwtToken(username);

            bool verification = _verifyLoginCodeVerification.ValidateCode(username, code);

            if(verification)
            {
                return new VerifyLoginCodeResponseDto()
                {
                    Token = token
                };
            }

            throw new Exception("Something went wrong");
        }

        public async Task<RegisterUserResponseDto> RegisterUser(CreateUserDto request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                throw new ArgumentException("Passwords do not match.");
            }

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Password cannot be empty.");
            }

            var permissionSet = await _dataContext.PermissionSets
                .FirstOrDefaultAsync(ps => ps.Name == "StandardUser");

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            byte[] emailHash = HashEmail(request.Email);

            var newUser = new User
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                LastName = request.LastName,
                UserName = request.UserName,
                PermissionSet = permissionSet,
                EmailHash = emailHash,
                IsVerified = false
            };

            _dataContext.Users.Add(newUser);
            permissionSet?.Users.Add(newUser);

            await _dataContext.SaveChangesAsync();

            string emailHashString = Convert.ToBase64String(emailHash);

            string emailText = _emailBody.createBody(emailHashString);

            var sendEmail = new MimeMessage();
            sendEmail.From.Add(MailboxAddress.Parse(newUser.Email));
            sendEmail.To.Add(MailboxAddress.Parse(newUser.Email));
            sendEmail.Subject = "Verify Email Address";
            sendEmail.Body = new TextPart("html")
            {
                Text = emailText
            };

            var client = new SmtpClient();

            client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            client.Authenticate("jamakovicemrah6@gmail.com", /*ovdje ide password*/);
            client.Send(sendEmail);
            client.Disconnect(true);

            return new RegisterUserResponseDto()
            {
                Response = "We have sent you a confirmation email! Please confirm email to finish your registration."
            };
        }

        public async Task<User> UpdateUser(UpdateUserDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");
            }

            var user = await GetSpecificUser(request.Id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            if(!request.Name.IsNullOrEmpty())
            {
                user.Name = request.Name ?? user.Name;
            }
            if (!request.Email.IsNullOrEmpty())
            {
                user.Email = request.Email ?? user.Email;
            }
            if (!request.LastName.IsNullOrEmpty())
            {
                user.LastName = request.LastName ?? user.LastName;
            }
            if (!request.UserName.IsNullOrEmpty())
            {
                user.UserName = request.UserName ?? user.UserName;
            }
            
            var permissionSet = await _dataContext.PermissionSets.Include(ps => ps.Users).FirstOrDefaultAsync(ps => ps.Id == request.permissionSetId);

            if (permissionSet == null)
            {
                throw new KeyNotFoundException("Permission set not found.");
            }

            if (user.PermissionSet != null && user.PermissionSet != permissionSet)
            {
                user.PermissionSet.Users.Remove(user);
                user.PermissionSet = permissionSet;
            }

            permissionSet.Users.Add(user);

            await _dataContext.SaveChangesAsync();

            return user;
        }

        public async Task<UpdatePasswordResponseDto> UpdatePassword(UpdateUserPasswordDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.ExistingPassword))
            {
                throw new ArgumentException("Invalid input parameters.");
            }

            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == request.Username);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            bool verification = VerifyPasswordHash(request.ExistingPassword, user.PasswordHash, user.PasswordSalt);
            if (!verification)
            {
                throw new UnauthorizedAccessException("Existing password is incorrect.");
            }

            CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _dataContext.SaveChangesAsync();

            return new UpdatePasswordResponseDto() 
            {
                Response = "Password Successfully Changed!"
            };
        }

        public async Task<UpdatePasswordResponseDto> ResetPassword(string emailHash, string newPassword)
        {
            string decodedEmailHash = HttpUtility.UrlDecode(emailHash);

            decodedEmailHash = decodedEmailHash.Replace(" ", "+");

            byte[] inputEmailHash = Convert.FromBase64String(decodedEmailHash);

            string username = _resetPasswordVerification.GetUsernameByEmailHash(inputEmailHash);

            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                throw new Exception("User does not exist!");
            }

            if (!user.IsVerified)
            {
                throw new Exception("User not verified!");
            }

            bool validation = _resetPasswordVerification.ValidateEmailHash(username, inputEmailHash);

            if(!validation)
            {
                throw new Exception("Something went wrong!");
            }

            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _dataContext.SaveChangesAsync();

            return new UpdatePasswordResponseDto() 
            {
                Response = "Password Successfully Changed!"
            };
        }

        public async Task<DeleteUserResponseDto> DeleteUser(int id)
        {
            var user = await GetSpecificUser(id);
            if (user == null)
            {
                throw new Exception("User with id:{id} does not exist!");
            }

            user.PermissionSet.Users.Remove(user);

            _dataContext.Users.Remove(user);

            await _dataContext.SaveChangesAsync();

            return new DeleteUserResponseDto() 
            {
                Response = "User Successfully Deleted!"
            };
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using (var hmac = new HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return computedHash.SequenceEqual(storedHash);
            }
        }

        public byte[] HashEmail(string email)
        {
            var saltedEmail = email + emailSalt;

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedEmail));
                return hashBytes;
            }
        }

        public bool VerifyEmail(string email, byte[] hash)
        {
            var saltedEmail = email + emailSalt;
            byte[] newHash = HashEmail(saltedEmail);

            return newHash == hash;
        }

        private string GenerateVerificationCode()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString(); // 6-digit code
        }
    }
}
