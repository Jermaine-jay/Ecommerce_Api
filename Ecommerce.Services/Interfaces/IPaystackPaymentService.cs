using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Services.Implementations;
using PayStack.Net;

namespace Ecommerce.Services.Interfaces
{
    public interface IPaystackPaymentService
    {
        Task<TransactionResponse> MakePayment(string userId, string orderId);
        Task<TransactionResponse> BankCharge(BankPaymentRequest request, string userId);
        Task<ChargeResponse> VerifyBankCharge(string refrence, string otp);
        Task<VerifyTransactionResponse> VerifyPayment(string referenceCode);
        Task<TransactionResponse> CardPayment(string userId, CardPaymentRequest request);
        Task<ResolveAccountResponse> GetAccount(string accountnumber, string bankcode);
        Task<List<BankResponse>> ListBank();
        Task<bool> IsServiceUpAsync();
    }
}
