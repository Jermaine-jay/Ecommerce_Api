using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Services.Interfaces;
using Ecommerce_Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderService _orderService;

        public OrderController(IHttpContextAccessor contextAccessor, IOrderService orderService)
        {
            _httpContextAccessor = contextAccessor;
            _orderService = orderService;
        }


        [HttpPost("clearcart", Name = "clearcart")]
        [SwaggerOperation(Summary = "clear all cart items")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "order", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User cart Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetUser()
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _orderService.ClearCart(userId);
            return Ok(response);
        }



        [HttpPost("createorder", Name = "createorder")]
        [SwaggerOperation(Summary = "user creates order")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "order", Type = typeof(OrderResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Product Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _orderService.CreateOrder(userId, request);
            return Ok(response);
        }



        [HttpGet("shippingaddress", Name = "shippingaddress")]
        [SwaggerOperation(Summary = " add shipping address to order")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "order", Type = typeof(OrderResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Product Not Found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> ShippingAddress([FromBody] OrderRequest request)
        {
            string? userId = _httpContextAccessor?.HttpContext?.User?.GetUserId();
            var response = await _orderService.CreateOrder(userId, request);
            return Ok(response);
        }
    }
}
