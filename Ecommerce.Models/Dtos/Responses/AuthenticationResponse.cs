namespace Ecommerce.Models.Dtos.Responses
{
    public class AuthenticationResponse
    {
        public JwtToken JwtToken { get; set; }
        public string UserType { get; set; }
        public string FullName { get; set; }
        public bool TwoFactor { get; set; }

        public bool IsExisting { get; set; }

    }

    public class JwtToken
    {
        public string Token { get; set; }
        public DateTime Issued { get; set; }
        public DateTime? Expires { get; set; }
    }
}
