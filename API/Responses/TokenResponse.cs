namespace API.Responses
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string TokenType { get; set; }
        public int ExpiresIn { get; set; }
    }
}