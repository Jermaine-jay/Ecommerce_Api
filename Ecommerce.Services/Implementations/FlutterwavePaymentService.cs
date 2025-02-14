using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Models.Enums;
using Ecommerce.Services.Extensions;
using Ecommerce.Services.Interfaces;
using Flutterwave.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AppConstants = Ecommerce.Services.Infrastructure.AppConstants;

namespace Ecommerce.Services.Implementations
{
    public class FlutterwavePaymentService : IFlutterwavePaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AppConstants _appConstants;
        private readonly IRepository<Order> _orderRepo;
        private readonly IConfiguration _configuration;
        private readonly FlutterwaveConfig _flutterwave;
        private readonly IServiceFactory _serviceFactory;
        private readonly UserManager<ApplicationUser> _userManager;

        public FlutterwavePaymentService(IUnitOfWork unitOfWork, IConfiguration configuration, IOrderService orderService,
            UserManager<ApplicationUser> userManager, IServiceFactory serviceFactory)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpClient = new HttpClient();
            _configuration = configuration;
            _serviceFactory = serviceFactory;
            _orderRepo = _unitOfWork.GetRepository<Order>();
            _appConstants = _serviceFactory.GetService<AppConstants>();
            _flutterwave = _serviceFactory.GetService<FlutterwaveConfig>();
        }

        public async Task<FlutterTransactionResponse> FlutterPayment(string userId, FlutterPaymentRequest request)
        {
            Order order = await _orderRepo.GetSingleByAsync(order => order.Id.ToString() == request.OrderId, include: u => u.Include(u => u.OrderItems))
                ?? throw new InvalidOperationException($"Order not found.");

            ApplicationUser user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException($"User not found.");

            FlutterwaveApi flutter = new FlutterwaveApi(_flutterwave.ApiKey);
            Currency currency = Flutterwave.Net.Currency.NigerianNaira;
            switch (request.Currency)
            {
                case (int)CountryCurrency.NigerianNaira:
                    currency = Flutterwave.Net.Currency.NigerianNaira;
                    break;
            }

            string reference = Guid.NewGuid().ToString();
            string PaymentDescription = $"Payment for {order.OrderItems.Count()} bought on Ecommerce website";
            string address = $"{order.ShippingAddress.HomeNumber} {order.ShippingAddress.Street} {order.ShippingAddress.City}";

            InitiatePaymentResponse result = flutter.Payments.InitiatePayment(reference, order.Total / 10, _appConstants.CallbackUrl,
                order.UserName, user.Email, address, _appConstants.PaymentTitle, PaymentDescription, currency.ToString());

            FlutterTransactionResponse response = new()
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
            FlutterwaveApi say = new FlutterwaveApi(_flutterwave.ApiKey);
            var result = say.Transactions.VerifyTransaction(int.Parse(transaction_id));

            Order order = await _orderRepo.GetSingleByAsync(order => order.Id.Equals(result.Data.TxRef))
                ?? throw new InvalidOperationException($"Order not found.");

            if (result.Status == "successful")
            {
                order.Paid = true;
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepo.UpdateAsync(order);
            }

            FlutterTransactionResponse response = new()
            {
                Message = result.Message,
                Status = result.Status,
                Reference = result.Data.TxRef,
            };

            return response;
        }

        public async Task<bool> IsServiceUpAsync()
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_flutterwave.ApiKey}");

            HttpResponseMessage response = await _httpClient.GetAsync(_appConstants.FlutterPingUrl);
            return response.IsSuccessStatusCode;
        }
    }
}
