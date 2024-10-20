namespace Diplomski.InputRequestDto
{
    public class UpdateUserPasswordDto
    {
        public string Username { get; set; } = string.Empty;
        public string ExistingPassword { get; set; } = string.Empty;

        public string NewPassword { get; set; } = string.Empty;
    }
}
