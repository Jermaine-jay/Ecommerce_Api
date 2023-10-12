using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;


namespace Ecommerce_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }



        [AllowAnonymous]
        [HttpPost("available-system", Name = "available-system")]
        [SwaggerOperation(Summary = "Check available payment platform")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "order", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "order", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "order", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CheckService()
        {
            var response = await _paymentService.AvailableSystem();
            if(response == "paystack")
                return RedirectToAction("paystackpayment", "Paystack");

            if(response == "flutter")
                return RedirectToAction("flutterwavepayment", "Flutterwave");

            return BadRequest(response);
        }
    }
}
