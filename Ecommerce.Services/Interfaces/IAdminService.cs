using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Services.Implementations;

namespace Ecommerce.Services.Interfaces
{
    public interface IAdminService
    {
        Task<SuccessResponse> UpdateCategory(string CategoryId, string name);
        Task<CreateCategoryResponse> CreateCategory(CreateCategoryRequest request);
        Task<SuccessResponse> GetUsers();
        Task<ApplicationUserDto> GetUser(string userId);
        Task<SuccessResponse> DeleteUser(string userId);
        Task<SuccessResponse> LockUser(LockUserRequest request);
        Task<SuccessResponse> GetAllCategories();
    }
}
