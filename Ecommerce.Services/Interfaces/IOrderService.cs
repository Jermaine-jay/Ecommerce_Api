using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Services.Implementations;

namespace Ecommerce.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> ShippingAddress(ShippingAddressRequest request);
        Task<OrderResponse> CreateOrder(string userId, OrderRequest request);
        Task<SuccessResponse> ClearCart(string userId);
    }
}
