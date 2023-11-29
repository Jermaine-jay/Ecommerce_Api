namespace Ecommerce.Models.Dtos.Responses
{
    public class ResetPasswordResponse
    {
        public string? Message { get; set; }
        public string? Token { get; set; }
        public bool Success { get; set; }
    }
}
