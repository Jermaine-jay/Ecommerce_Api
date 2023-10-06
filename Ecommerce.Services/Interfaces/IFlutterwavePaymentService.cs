using Ecommerce.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Services.Interfaces
{
    public interface IFlutterwavePaymentService
    {
        Task<object> FlutterPayment(FlutterPaymentRequest request);
        Task<object> VerifyFlutterPayment([FromQuery] string transaction_id);
        Task<bool> IsServiceUpAsync();
    }
}
