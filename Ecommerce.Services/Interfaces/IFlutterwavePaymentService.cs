using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;

namespace Ecommerce.Services.Interfaces
{
    public interface IFlutterwavePaymentService
    {
        Task<FlutterTransactionResponse> FlutterPayment(string userId, FlutterPaymentRequest request);
        Task<FlutterTransactionResponse> VerifyFlutterPayment(string transaction_id);
        Task<bool> IsServiceUpAsync();
    }
}
