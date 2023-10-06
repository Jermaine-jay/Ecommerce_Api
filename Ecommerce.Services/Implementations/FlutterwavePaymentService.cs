using Ecommerce.Models.Enums;
using Ecommerce.Services.Interfaces;
using Flutterwave.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;

namespace Ecommerce.Services.Implementations
{
    public class FlutterwavePaymentService :IFlutterwavePaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        private readonly string _Apikey;
        public FlutterwavePaymentService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _configuration = configuration;
            _Apikey = _configuration["Flutterwave:ApiKey"];
        }


        public async Task<object> FlutterPayment(FlutterPaymentRequest request)
        {

            var flutter = new FlutterwaveApi(_Apikey);
            var currency = Flutterwave.Net.Currency.NigerianNaira;
            switch (request.Currency)
            {
                case (int)CountryCurrency.NigerianNaira:
                    currency = Flutterwave.Net.Currency.NigerianNaira;
                    break;
            }

            var result = flutter.Payments.InitiatePayment(Guid.NewGuid().ToString(), request.Amount, request.CallbackUrl, request.Fullname, request.Email, request.Phonenumber, request.PaymentTitle, request.PaymentDescription, currency.ToString());

            return result;
        }


        public async Task<object> VerifyFlutterPayment([FromQuery] string transaction_id)
        {
            var say = new FlutterwaveApi(_Apikey);
            var verify = say.Transactions.VerifyTransaction(int.Parse(transaction_id));
            if (verify.Status == "successful")
            {
                return true;
            }
            return verify;
        }


        public async Task<bool> IsServiceUpAsync()
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_Apikey}");

            var response = await _httpClient.GetAsync("https://api.flutterwave.com/v3//ping");
            return response.IsSuccessStatusCode;
        }
    }


    public class FlutterPaymentRequest
    {

        [Required]
        public int Amount { get; set; }
        [Required]
        public string Email { get; set; }
        public string? Fullname { get; set; }
        public string? Phonenumber { get; set; }
        public string PaymentTitle { get; set; }
        public string PaymentDescription { get; set; }
        public string CallbackUrl { get; set; } = "https://Localhost:7085/api/Flutter/FlutterVerify";
        public int Currency { get; set; }
    }
}
