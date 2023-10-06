using Ecommerce.Models.Dtos.Requests;
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
        [HttpPost("system", Name = "system")]
        [SwaggerOperation(Summary = "register user")]
        [SwaggerResponse(StatusCodes.Status200OK)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CheckService()
        {
            var response = await _paymentService.AvailableSystem();
            return Ok(response);
        }
    }
}
