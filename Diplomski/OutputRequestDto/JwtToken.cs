namespace Diplomski.OutputRequestDto
{
    public class JwtToken
    {

        public required string Token { get; set; }

        public DateTime? ExipresAt { get; set; }
    }
}
