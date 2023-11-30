using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.Dtos.Requests
{
    public class ForgotPasswordRequest
    {
        [Required, DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
    }
}
