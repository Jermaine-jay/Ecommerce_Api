using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Services.Interfaces;
using Ecommerce_Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce_Api.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "Authorization")]
    [ApiController]
    public class FlutterwaveController : ControllerBase
    {
        private readonly IFlutterwavePaymentService _flutterwavePaymentService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public FlutterwaveController(IFlutterwavePaymentService flutterwavePaymentService, IHttpContextAccessor httpContextAccessor)
        {
            _flutterwavePaymentService = flutterwavePaymentService;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpPost("flutterwavepayment", Name = "flutterwavepayment")]
        [SwaggerOperation(Summary = "flutterwave payment system")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Transaction Details", Type = typeof(FlutterTransactionResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Transaction Details", Type = typeof(TransactionResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Transaction Details", Type = typeof(TransactionResponse))]
        public async Task<IActionResult> FlutterwavePayment([FromForm] FlutterPaymentRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _flutterwavePaymentService.FlutterPayment(userId, request);
            if (response.Status == "successful")
                return RedirectToAction(nameof(VerifyFlutterPayment));

            return BadRequest(response);

        }


    
        [HttpGet("verifyflutterwavepayment", Name = "verifyflutterwavepayment")]
        [SwaggerOperation(Summary = "verify flutterwave payment system")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Transaction Details", Type = typeof(FlutterTransactionResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Transaction Details", Type = typeof(TransactionResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "Transaction Details", Type = typeof(TransactionResponse))]
        public async Task<IActionResult> VerifyFlutterPayment([FromQuery] string transaction_id)
        {
            var response = await _flutterwavePaymentService.VerifyFlutterPayment(transaction_id);
            return Ok(response);
        }
    }
}
