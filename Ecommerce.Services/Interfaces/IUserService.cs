using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Services.Implementations;

namespace Ecommerce.Services.Interfaces
{
    public interface IUserService
    {
        Task<SuccessResponse> DeleteFromCart(string userId, string cartitemId);
        Task<CartItemResponse> AddToCart(string userId, AddToCartRequest request);
        Task<CartResponse> GetCart(string userId);
        Task<SuccessResponse> UpdateAccount(string userId, UpdateUserRequest request);
        Task<ProfileResponse> Profile(string userId);
        Task<SuccessResponse> DeleteAccount(string userId);
        Task<SuccessResponse> ChangePassword(string userId, ChangePasswordRequest request);
    }
}
