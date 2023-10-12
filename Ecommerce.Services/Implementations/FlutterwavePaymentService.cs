using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Models.Enums;
using Ecommerce.Services.Interfaces;
using Flutterwave.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Ecommerce.Services.Implementations
{
    public class FlutterwavePaymentService : IFlutterwavePaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IRepository<Order> _orderRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _Apikey;

        public FlutterwavePaymentService(IUnitOfWork unitOfWork, IConfiguration configuration, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _httpClient = new HttpClient();
            _configuration = configuration;
            _Apikey = _configuration["Flutterwave:ApiKey"];
            _orderRepo = _unitOfWork.GetRepository<Order>();
            _userManager = userManager;
        }


        public async Task<FlutterTransactionResponse> FlutterPayment(string userId, FlutterPaymentRequest request)
        {
            var order = await _orderRepo.GetSingleByAsync(order => order.Id.ToString() == request.OrderId, include: u=> u.Include(u=>u.OrderItems))
                ?? throw new InvalidOperationException($"Order not found.");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException($"User not found.");

            var flutter = new FlutterwaveApi(_Apikey);
            var currency = Flutterwave.Net.Currency.NigerianNaira;
            switch (request.Currency)
            {
                case (int)CountryCurrency.NigerianNaira:
                    currency = Flutterwave.Net.Currency.NigerianNaira;
                    break;
            }

            var CallbackUrl = "https://Localhost:7085/api/Flutterwave/verifyflutterwavepayment";
            var PaymentTitle = "Ecommerce Payment";
            var PaymentDescription = $"Payment for {order.OrderItems.Count()} bought on Ecommerce website";
            var reference = Guid.NewGuid().ToString();
            var address = $"{order.ShippingAddress.HomeNumber} {order.ShippingAddress.Street} {order.ShippingAddress.City}";

            var result = flutter.Payments.InitiatePayment(reference, order.Total/10, CallbackUrl,
                order.UserName, user.Email, address, PaymentTitle, PaymentDescription, currency.ToString());

            var response = new FlutterTransactionResponse
            {
                Message = result.Message,
                Status = result.Status,
                Reference = reference,
                AuthorizationUrl = result.Data.Link,
            };

            order.Txnref = reference;
            await _orderRepo.UpdateAsync(order);

            return response;
        }


        public async Task<FlutterTransactionResponse> VerifyFlutterPayment(string transaction_id)
        {

            var say = new FlutterwaveApi(_Apikey);
            var result = say.Transactions.VerifyTransaction(int.Parse(transaction_id));

            var order = await _orderRepo.GetSingleByAsync(order => order.Id.Equals(result.Data.TxRef))
                ?? throw new InvalidOperationException($"Order not found.");

            if (result.Status == "successful")
            {
                order.Paid = true;
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepo.UpdateAsync(order);
            }

            var response = new FlutterTransactionResponse
            {
                Message = result.Message,
                Status = result.Status,
                Reference = result.Data.TxRef,
            };

            return response;
        }


        public async Task<bool> IsServiceUpAsync()
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_Apikey}");

            var response = await _httpClient.GetAsync("https://api.flutterwave.com/v3//ping");
            return response.IsSuccessStatusCode;
        }
    }  
}
