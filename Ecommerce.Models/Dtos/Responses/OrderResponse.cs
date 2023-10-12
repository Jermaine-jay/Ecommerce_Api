using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Models.Dtos.Responses
{
    public class OrderResponse
    {
        public string Id { get; set; }
        public string OrderDate { get; set; }
        public string? UserName { get; set; }
        public decimal Total { get; set; }
        public string Received { get; set; }
        public string Paid { get; set; }
        public string HomeNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string? Postcode { get; set; }
        public string Country { get; set; }
    }
}
