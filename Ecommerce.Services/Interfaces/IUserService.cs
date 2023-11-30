using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;

namespace Ecommerce.Services.Interfaces
{
    public interface IUserService
    {
        Task<SuccessResponse> DeleteFromCart(string userId, string cartitemId);
        Task<SuccessResponse> DeleteCartItems(string userId);
        Task<CartItemResponse> AddToCart(string userId, AddToCartRequest request);
        Task<Cart> GetCart(string userId);
        Task<SuccessResponse> UpdateAccount(string userId, UpdateUserRequest request);
        Task<ProfileResponse> Profile(string userId);
        Task<SuccessResponse> DeleteAccount(string userId);
        Task<SuccessResponse> ChangePassword(string userId, ChangePasswordRequest request);
    }
}
