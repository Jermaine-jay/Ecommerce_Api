using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Models.Dtos.Requests
{
    public class CardPaymentRequest
    {
        public string? Plan { get; set; }
        public string? CardNumber { get; set; }
        public string? Cvv { get; set; }
        public string? Pin { get; set; }
        public string? ExipiryMonth { get; set; }
        public string? ExipiryYear { get; set; }
        public string? Currency { get; set; } = "NGN";
        public string? OrderId { get; set; }
    }
}
