namespace Lulu_Portfolio.API.Models.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }
    }
}