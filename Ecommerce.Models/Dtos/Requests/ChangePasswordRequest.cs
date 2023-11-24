using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.Dtos.Requests
{
    public class ChangePasswordRequest
    {

        [Required, DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Required, DataType(DataType.Password)]
        public string? NewPassword { get; set; }
    }
}
