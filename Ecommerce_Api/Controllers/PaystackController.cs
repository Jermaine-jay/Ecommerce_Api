using Ecommerce.Services.Implementations;
using Ecommerce.Services.Interfaces;
using Ecommerce_Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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


        [AllowAnonymous]
        [HttpPost("paystackpayment", Name = "paystackpayment")]
        [SwaggerOperation(Summary = "paystack payment system")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PaystackPayment([FromBody] DepositRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _paystackPaymentService.MakePayment(userId, request);
            if (response.Status)
                return RedirectToAction(nameof(VerifyPaystackPayment));

            return BadRequest(response);

        }


        [AllowAnonymous]
        [HttpGet("verifypaystackpayment", Name = "verifypaystackpayment")]
        [SwaggerOperation(Summary = "verify paystack payment system")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VerifyPaystackPayment([FromBody] string refrenceCode)
        {
            var response = await _paystackPaymentService.VerifyPayment(refrenceCode);
            return Ok(response);
        }
    }
}
