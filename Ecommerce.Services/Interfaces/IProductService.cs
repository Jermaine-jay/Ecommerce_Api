using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Services.Implementations;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Services.Interfaces
{
    public interface IProductService
    {
        Task<SuccessResponse> UpdateStock(string productId, int stock);
        Task<SuccessResponse> UpdateProduct(UpdateProductRequest request);
        Task<CreateProductResponse> CreateProduct(CreateProductRequest request);
        Task<object> AddImages(Guid productId, List<IFormFile> files);
        Task<SuccessResponse> DeleteImage(string publicId);
        Task<IEnumerable<Product>> GetProductsAsync();
        Task<ProductDto> GetProductAsync(string productId);
        Task<SuccessResponse> AddVariations(ProductVarionRequest request);

    }
}
