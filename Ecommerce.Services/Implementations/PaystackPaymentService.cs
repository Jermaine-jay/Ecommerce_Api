using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
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
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly string _secret;
        private readonly HttpClient _httpClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Order> _orderRepo;


        public PaystackPaymentService(IConfiguration configuration, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _secret = _configuration["Paystack:ApiKey"];
            _orderRepo = _unitOfWork.GetRepository<Order>();
            _httpClient = new HttpClient();
        }


        public async Task<TransactionResponse> MakePayment(string userId, string orderId)
        {
            var order = await _orderRepo.GetSingleByAsync(order => order.Id.ToString() == orderId)
                ?? throw new InvalidOperationException($"Order not found.");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException($"User not found.");

            var amount = Convert.ToInt32((order.Total * 10));
            var Request = new TransactionInitializeRequest
            {
                Reference = Guid.NewGuid().ToString(),
                Bearer = user.FirstName,
                Email = user.Email,
                AmountInKobo = amount,
                TransactionCharge = 10,
                Currency = "NGN",
                CallbackUrl = "https://localhost:7076//api/Paystack/verifypaystackpayment",
            };

            PayStackApi payStack = new(_secret);
            var result = payStack.Transactions.Initialize(Request);

            var response = new TransactionResponse
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
            var order = await _orderRepo.GetAllAsync()
                  ?? throw new InvalidOperationException("Not Found");
            var o = order.Where(o => o.Txnref.ToString() == referenceCode).FirstOrDefault();

            PayStackApi payStack = new(_secret);
            TransactionVerifyResponse result = payStack.Transactions.Verify(referenceCode);
            if (result.Data.Status == "success")
            {
                o.Paid = true;
                o.UpdatedAt = DateTime.UtcNow;
                await _orderRepo.UpdateAsync(o);
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
            var order = await _orderRepo.GetSingleByAsync(order => order.Id.ToString() == request.OrderId)
                ?? throw new InvalidOperationException($"Order not found.");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException($"User not found.");

            PayStackApi payStack = new(_secret);

            var bankChargeRequest = new BankChargeRequest
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

            var result = payStack.Charge.ChargeBank(bankChargeRequest, makeReferenceUnique: false);
            var response = new TransactionResponse
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
            var order = await _orderRepo.GetSingleByAsync(order => order.Txnref.ToString() == refrence)
              ?? throw new InvalidOperationException($"Order not found.");

            PayStackApi payStack = new(_secret);
            var result = payStack.Charge.SubmitOTP(refrence, otp);
            if (result.Status)
            {
                order.Paid = true;
                order.UpdatedAt = DateTime.UtcNow;
                await _orderRepo.UpdateAsync(order);
                return result;
            }
            return result;
        }

        public async Task<TransactionResponse> CardPayment(string userId, CardPaymentRequest request)
        {
            var order = await _orderRepo.GetSingleByAsync(order => order.Id.ToString() == request.OrderId)
                ?? throw new InvalidOperationException($"Order not found.");

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException($"User not found.");

            PayStackApi payStack = new PayStackApi(_secret);
            var cardChargeRequest = new CardChargeRequest
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
            var result = payStack.Charge.ChargeCard(cardChargeRequest, makeReferenceUnique: false);
            var response = new TransactionResponse
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
            PayStackApi payStack = new(_secret);
            var result = payStack.Miscellaneous.ResolveAccountNumber(accountnumber, bankcode);
            var response = new ResolveAccountResponse
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
            PayStackApi payStack = new(_secret);
            var result = payStack.Miscellaneous.ListBanks();
            var response = result.Data.Select(u => new BankResponse
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

            var response = await _httpClient.GetAsync("/healthcheck");

            PayStackApi payStack = new(_secret);

            return response.IsSuccessStatusCode;
        }
    }
}
