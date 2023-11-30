using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.Dtos.Requests
{
    public class ResetPasswordRequest
    {
        [Required]
        public string? Token { get; set; }

        [Required, DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Required, DataType(DataType.Password), Compare(nameof(NewPassword))]
        public string? ConfirmPassword { get; set; }
    }
}
