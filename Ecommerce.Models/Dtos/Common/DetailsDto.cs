using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Models.Dtos.Common
{
    public class DetailsDto
    {
        public string? UserId { get; set; }

        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password), Compare(nameof(Password))]
        public string? ConfirmNewPassword { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
    }
}
