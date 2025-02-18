using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Services.Extensions;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PayStack.Net;
using Sprache;
using Order = Ecommerce.Models.Entities.Order;


namespace Ecommerce.Services.Implementations
{
    public class PaystackPaymentService : IPaystackPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaystackConfig _paystack;
        private readonly IRepository<Order> _orderRepo;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaystackPaymentService(IConfiguration configuration, IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager, PaystackConfig paystack)
        {
            _paystack = paystack;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _httpClient = new HttpClient();
            _configuration = configuration;
            _orderRepo = _unitOfWork.GetRepository<Order>();
        }

        public async Task<TransactionResponse> MakePayment(string userId, string orderId)
        {
            Order order = await _orderRepo.GetSingleByAsync(order => order.Id.ToString() == orderId)
                ?? throw new InvalidOperationException($"Order not found.");

            ApplicationUser user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException($"User not found.");

            int amount = Convert.ToInt32((order.Total * 10));
            TransactionInitializeRequest Request = new TransactionInitializeRequest
            {
                Reference = Guid.NewGuid().ToString(),
                Bearer = user.FirstName,
                Email = user.Email,
                AmountInKobo = amount,
                TransactionCharge = 10,
                Currency = "NGN",
                CallbackUrl = "https://localhost:7076//api/Paystack/verifypaystackpayment",
            };

            PayStackApi payStack = new(_paystack.ApiKey);
            TransactionInitializeResponse result = payStack.Transactions.Initialize(Request);

            TransactionResponse response = new()
            {
                Message = result.Message,
                Status = result.Status,
                Reference = result.Data.Reference,
                AuthorizationUrl = result.Data.AuthorizationUrl,
            };

            order.Txnref = result.Data.Reference;
            await _orderRepo.UpdateAsync(order);

            return response;
        }

        public async Task<VerifyTransactionResponse> VerifyPayment(string referenceCode)
        {
            IEnumerable<Order> orders = await _orderRepo.GetAllAsync()
                  ?? throw new InvalidOperationException("Not Found");
            Order? order = orders.Where(o => o.Txnref.ToString() == referenceCode).SingleOrDefault();

            PayStackApi payStack = new(_paystack.ApiKey);
            TransactionVerifyResponse result = payStack.Transactions.Verify(referenceCode);
            if (result.Data.Status == "success")
            {
                order.Paid = true;
                order.UpdatedAt = DateTime.Now;
                await _orderRepo.UpdateAsync(order);
            }

            var response = new VerifyTransactionResponse
            {
                Message = result.Message,
                Status = result.Status,
                Reference = result.Data.Reference,
                DataStatus = result.Data.Status,
                Amount = result.Data.Amount,
            };

            return response;
        }

        public async Task<TransactionResponse> BankCharge(BankPaymentRequest request, string userId)
        {
            Order order = await _orderRepo.GetSingleByAsync(order => order.Id.ToString() == request.OrderId)
                ?? throw new InvalidOperationException($"Order not found.");

            ApplicationUser user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException($"User not found.");

            PayStackApi payStack = new(_paystack.ApiKey);

            BankChargeRequest bankChargeRequest = new BankChargeRequest
            {
                Email = user.Email,
                Amount = order.Total.ToString(),
                Bank = new Bank
                {
                    Code = request.Code,
                    AccountNumber = request.AccountNumber
                },
                Birthday = request.Birthday,
                Reference = Guid.NewGuid().ToString(),
            };

            ChargeResponse result = payStack.Charge.ChargeBank(bankChargeRequest, makeReferenceUnique: false);
            TransactionResponse response = new TransactionResponse
            {
                Message = result.Message,
                Status = result.Status,
                Reference = result.Data.Reference,
            };

            order.Txnref = result.Data.Reference;
            await _orderRepo.UpdateAsync(order);

            return response;
        }

        public async Task<ChargeResponse> VerifyBankCharge(string refrence, string otp)
        {
            Order order = await _orderRepo.GetSingleByAsync(order => order.Txnref.ToString() == refrence)
              ?? throw new InvalidOperationException($"Order not found.");

            PayStackApi payStack = new(_paystack.ApiKey);
            ChargeResponse result = payStack.Charge.SubmitOTP(refrence, otp);
            if (result.Status)
            {
                order.Paid = true;
                order.UpdatedAt = DateTime.Now;
                await _orderRepo.UpdateAsync(order);
                return result;
            }

            return result;
        }

        public async Task<TransactionResponse> CardPayment(string userId, CardPaymentRequest request)
        {
            Order order = await _orderRepo.GetSingleByAsync(order => order.Id.ToString() == request.OrderId)
                ?? throw new InvalidOperationException($"Order not found.");

            ApplicationUser user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException($"User not found.");

            PayStackApi payStack = new PayStackApi(_paystack.ApiKey);
            CardChargeRequest cardChargeRequest = new CardChargeRequest
            {
                Email = user.Email,
                Amount = order.Total.ToString(),
                Pin = request.Pin,
                Card = new Card
                {
                    Cvv = request.Cvv,
                    Number = request.CardNumber,
                    ExpiryMonth = request.ExipiryMonth,
                    ExpiryYear = request.ExipiryYear
                },

                Reference = Guid.NewGuid().ToString(),
            };

            ChargeResponse result = payStack.Charge.ChargeCard(cardChargeRequest, makeReferenceUnique: false);
            TransactionResponse response = new TransactionResponse
            {
                Message = result.Message,
                Status = result.Status,
                Reference = result.Data.Reference,
            };

            if (result.Data.Status == "success")
            {
                order.Txnref = result.Data.Reference;
                order.Paid = true;
                await _orderRepo.UpdateAsync(order);
            }

            return response;
        }

        public async Task<ResolveAccountResponse> GetAccount(string accountnumber, string bankcode)
        {
            PayStackApi payStack = new(_paystack.ApiKey);
            ResolveAccountNumberResponse result = payStack.Miscellaneous.ResolveAccountNumber(accountnumber, bankcode);
            ResolveAccountResponse response = new()
            {
                Status = result.Status,
                Message = result.Message,
                AccountName = result.Data.AccountName,
                AccountNumber = result.Data.AccountNumber,
            };

            return response;
        }

        public async Task<List<BankResponse>> ListBank()
        {
            PayStackApi payStack = new(_paystack.ApiKey);
            ListBanksResponse result = payStack.Miscellaneous.ListBanks();
            List<BankResponse> response = result.Data.Select(u => new BankResponse
            {
                BankName = u.Name,
                BankCode = u.Code,
                Active = u.Active,
            }).ToList();
            return response;
        }

        public async Task<bool> IsServiceUpAsync()
        {
            _httpClient.BaseAddress = new Uri("https://api.paystack.co/");
            HttpResponseMessage response = await _httpClient.GetAsync("/healthcheck");

            PayStackApi payStack = new(_paystack.ApiKey);
            return response.IsSuccessStatusCode;
        }
    }
}
