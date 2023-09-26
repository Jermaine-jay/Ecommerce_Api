using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models.Dtos.Requests
{
    public class UserRegistrationRequest
    {
        [Required]
        public string Firstname { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

    }
}
