using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Services.Implementations;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Ecommerce_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpPost("create-product", Name = "create-product")]
        [SwaggerOperation(Summary = "create new product ")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "Product", Type = typeof(CreateProductResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Category doesn't exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Product Name already exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {

            var response = await _productService.CreateProduct(request);
            return Ok(response);
        }


        [HttpPut("update-product", Name = "update-product")]
        [SwaggerOperation(Summary = "update existing product ")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "Product", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Product doesn't exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "category does not exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest request)
        {

            var response = await _productService.UpdateProduct(request);
            return Ok(response);
        }


        [HttpDelete("delete-product", Name = "delete-product")]
        [SwaggerOperation(Summary = "delete existing product ")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "true", Type = typeof(ProductUpdateResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "product doesn't exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "No image found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> UpdateProduct(string productId)
        {

            var response = await _productService.DeleteProduct(productId);
            return Ok(response);
        }


        [AllowAnonymous]
        [HttpPost("addvariation", Name = "addvariation")]
        [SwaggerOperation(Summary = "add product varieties")]
        [SwaggerResponse(StatusCodes.Status201Created, Description = "true", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "product doesn't exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "File cannot be empty", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> AddVariations([FromForm] ProductVarionRequest request)
        {
            var response = await _productService.AddVariations(request);
            return Ok(response);
        }


        [HttpDelete("delete-image", Name = "delete-image")]
        [SwaggerOperation(Summary = "delete product image")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "true", Type = typeof(SuccessResponse))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "product doesn't exist", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "File cannot be empty", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> DeleteImage(string publicId)
        {

            var response = await _productService.DeleteImage(publicId);
            return Ok(response);
        }


        [HttpGet("all-products", Name = "all-products")]
        [SwaggerOperation(Summary = "get all product")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "products", Type = typeof(Product))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "No product found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetProducts()
        {
            var response = await _productService.GetProductsAsync();
            return Ok(response);
        }


        [HttpGet("product", Name = "product")]
        [SwaggerOperation(Summary = "get product")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "product", Type = typeof(ProductDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "No product found", Type = typeof(ErrorResponse))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, Description = "It's not you, it's us", Type = typeof(ErrorResponse))]
        public async Task<IActionResult> GetProduct(string productId)
        {

            var response = await _productService.GetProductAsync(productId);
            return Ok(response);
        }
    }
}
