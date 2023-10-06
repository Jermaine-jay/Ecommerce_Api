using Ecommerce.Services.Implementations;
using PayStack.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Services.Interfaces
{
    public  interface IPaystackPaymentService
    {
        Task<TransactionInitializeResponse> MakePayment(string userId, DepositRequest depositRequest);
        Task<TransactionVerifyResponse> VerifyPayment(string referenceCode);
        Task<object> CardPayment(string userId, CardPaymentRequest request);
        Task<bool> IsServiceUpAsync();
    }
}
