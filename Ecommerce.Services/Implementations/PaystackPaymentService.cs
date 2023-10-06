using Ecommerce.Models.Entities;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PayStack.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Services.Implementations
{
    public class PaystackPaymentService : IPaystackPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _secret;
        private readonly HttpClient _httpClient;


        public PaystackPaymentService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _secret = _configuration["Paystack:ApiKey"];
            _httpClient = new HttpClient();
        }


        public async Task<TransactionInitializeResponse> MakePayment(string userId, DepositRequest depositRequest)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var Request = new TransactionInitializeRequest
            {
                Reference = Guid.NewGuid().ToString(),
                Bearer = user.FirstName,
                Email = user.Email,
                AmountInKobo = (depositRequest.AmountInKobo * 100),
                TransactionCharge = depositRequest.TransactionCharge,
                Currency = depositRequest.Currency,
                CallbackUrl = "https://localhost:7076//api/Paystack/verifypaystackpayment",
            };

            PayStackApi payStack = new(_secret);

            var result = payStack.Transactions.Initialize(Request);
            return result;
        }


        public async Task<TransactionVerifyResponse> VerifyPayment(string referenceCode)
        {
            PayStackApi payStack = new(_secret);
            TransactionVerifyResponse result = payStack.Transactions.Verify(referenceCode);
            return result;
        }


        public async Task<object> CardPayment(string userId, CardPaymentRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);

            PayStackApi payStack = new PayStackApi(_secret);

            var card = payStack.Charge.ChargeCard(user.Email, request.Amount, request.CardNumber, request.Cvv, request.ExipiryMonth, request.ExipiryYear, request.Pin);
            var response = JsonConvert.SerializeObject(card.RawJson);
            var result = JsonConvert.DeserializeObject<dynamic>(response);
            return result;
        }


        public async Task<bool> IsServiceUpAsync()
        {
            _httpClient.BaseAddress = new Uri("https://api.paystack.co/");
       
            var response = await _httpClient.GetAsync("/healthcheck");
            return response.IsSuccessStatusCode;
        }
    }

    public class DepositRequest
    {

        [Required]
        public int AmountInKobo { get; set; }
        public string? Plan { get; set; }
        public string SubAccount { get; set; }
        public int TransactionCharge { get; set; }
        public string Currency { get; set; } = "NGN";
    }


    public class CardPaymentRequest
    {

        [Required]
        public string Amount { get; set; }
        [Required]
        public string Email { get; set; }
        public string? Plan { get; set; }
        public string CardNumber { get; set; }
        public string Cvv { get; set; }
        public string Pin { get; set; }
        public string ExipiryMonth { get; set; }
        public string ExipiryYear { get; set; }
        public string Currency { get; set; } = "NGN";
    }
}
