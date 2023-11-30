using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Models.Dtos.Requests
{
    public class OrderRequest
    {
        public int Quantity { get; set; }
        public string? VariationId { get; set; }
    }
}
