using Ecommerce.Services.Interfaces;
using PayStack.Net;


namespace Ecommerce.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaystackPaymentService _paystackPaymentService;
        private readonly IFlutterwavePaymentService _flutterwavePaymentService;
        public PaymentService(IPaystackPaymentService paystackPaymentService, IFlutterwavePaymentService flutterwavePaymentService)
        {
            _paystackPaymentService = paystackPaymentService;
            _flutterwavePaymentService = flutterwavePaymentService;
        }


        public async Task<string> AvailableSystem()
        {
            var payment = "";

            var paystack =  _paystackPaymentService.IsServiceUpAsync();
            var flutter =  _flutterwavePaymentService.IsServiceUpAsync();

            await Task.WhenAny(paystack, flutter);

           if(paystack.IsCompletedSuccessfully) { return "paystack"; }
           if(flutter.IsCompletedSuccessfully) { return "flutter"; }



           return null;
        }
    }


}
