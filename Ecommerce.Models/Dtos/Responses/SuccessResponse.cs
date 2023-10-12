using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Models.Dtos.Responses
{
    public class SuccessResponse
    {
        public bool Success { get; set; }
        public object Data { get; set; }
    }
}
