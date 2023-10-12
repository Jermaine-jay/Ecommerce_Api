using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Services.Interfaces;
using Ecommerce_Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using PayStack.Net;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaystackController : ControllerBase
    {
        private readonly IPaystackPaymentService _paystackPaymentService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public PaystackController(IPaystackPaymentService paystackPaymentService, IHttpContextAccessor httpContextAccessor)
        {
            _paystackPaymentService = paystackPaymentService;
            _httpContextAccessor = httpContextAccessor;
        }


        //[AllowAnonymous]
        [HttpPost("paystackpayment", Name = "paystackpayment")]
        [SwaggerOperation(Summary = "paystack payment system")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Transaction Details", Type = typeof(TransactionResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Order not found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User not found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Internal Error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> PaystackCardPayment(string orderId)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _paystackPaymentService.MakePayment(userId, orderId);
            if (response.Status)
                return RedirectToAction(nameof(VerifyPaystackCardPayment));

            return Ok(response);

        }


        //[AllowAnonymous]
        [HttpGet("verifypaystackpayment", Name = "verifypaystackpayment")]
        [SwaggerOperation(Summary = "verify paystack payment system")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Verification Details", Type = typeof(VerifyTransactionResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Order not found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Internal Error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> VerifyPaystackCardPayment([FromQuery] string reference)
        {
            var response = await _paystackPaymentService.VerifyPayment(reference);
            return Ok(response);
        }



        //[AllowAnonymous]
        [HttpPost("paystack-bank-charge", Name = "paystack-bank-charge")]
        [SwaggerOperation(Summary = "paystack bankchargepayment payment system")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Transaction Details", Type = typeof(ChargeResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Order not found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "User not found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Internal Error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> PaystackBankCharge([FromBody] BankPaymentRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _paystackPaymentService.BankCharge(request, userId);
            if (response.Status)
                return RedirectToAction(nameof(VerifyPaystackBankCharge));

            return Ok(response);
        }



        //[AllowAnonymous]
        [HttpGet("verifybankcharge", Name = "verifybankcharge")]
        [SwaggerOperation(Summary = "verify paystack bank charge payment")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Transaction Details", Type = typeof(ChargeResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Order not found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "InternL Error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> VerifyPaystackBankCharge(string refrence, string otp)
        {
            var response = await _paystackPaymentService.VerifyBankCharge(refrence, otp);
            return Ok(response);
        }



        //[AllowAnonymous]
        [HttpPost("directcardpayment", Name = "directcardpayment")]
        [SwaggerOperation(Summary = "direct card payment")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Transaction Details", Type = typeof(TransactionResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Order not found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "InternL Error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DirectCardPayment([FromBody] CardPaymentRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _paystackPaymentService.CardPayment(userId, request);
            return Ok(response);
        }



        //[AllowAnonymous]
        [HttpGet("available-banks", Name = "available-banks")]
        [SwaggerOperation(Summary = "list available banks")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Banks Details", Type = typeof(BankResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Internal error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> ListBank()
        {
            var response = await _paystackPaymentService.ListBank();
            return Ok(response);
        }



        //[AllowAnonymous]
        [HttpPost("get-account", Name = "get-account")]
        [SwaggerOperation(Summary = "get existing account")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "account Details", Type = typeof(ResolveAccountResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Internal error", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetAccount(string accountnumber, string bankcode)
        {
            var response = await _paystackPaymentService.GetAccount(accountnumber, bankcode);
            return Ok(response);
        }
    }
}
