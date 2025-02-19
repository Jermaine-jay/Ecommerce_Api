using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Net;


namespace Ecommerce.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<ApplicationUser> _userRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _categoryRepo = _unitOfWork.GetRepository<Category>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
        }

        public async Task<CreateCategoryResponse> CreateCategory(CreateCategoryRequest request)
        {
            Category category = await _categoryRepo.GetSingleByAsync(p => p.Name == request.Name.ToLower());
            if (category != null)
                throw new InvalidOperationException("Category Name already exist");

            Category newCategory = new Category
            {
                Name = request.Name.ToLower(),
            };

            await _categoryRepo.AddAsync(newCategory);
            return new CreateCategoryResponse
            {
                Message = "Category Created",
                Status = HttpStatusCode.Created,
                Data = newCategory,
            };
        }


        public async Task<SuccessResponse> UpdateCategory(string CategoryId, string name)
        {
            Category category = await _categoryRepo.GetSingleByAsync(u => u.Id.ToString() == CategoryId) ??
                throw new InvalidOperationException("Category does not exist");

            category.Name = name.ToLower();
            category.UpdatedAt = DateTime.Now;

            await _categoryRepo.UpdateAsync(category);
            return new SuccessResponse
            {
                Success = true,
                Data = category.Name
            };
        }

        public async Task<SuccessResponse> GetAllCategories()
        {
            IEnumerable<Category> categories = await _categoryRepo.GetAllAsync()
                ?? throw new InvalidOperationException("No Category Found");

            return new SuccessResponse
            {
                Success = true,
                Data = categories
            };
        }

        public async Task<SuccessResponse> GetUsers()
        {
            IEnumerable<ApplicationUser> users = await _userRepo.GetAllAsync();
            if (users == null)
                throw new InvalidOperationException("Users Not Found");

            IEnumerable<ApplicationUserDto> result = users.Select(user => new ApplicationUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Active = user.Active ? "Active" : "Not Active",
                EmailConfirmed = user.EmailConfirmed ? "Confirmed" : "Not Confirmed",
                LockedOut = user.LockoutEnd?.ToString("dd MMMM yyyy"),
                CreatedAt = user.CreatedAt.ToString("dd MMMM yyyy"),
            });

            return new SuccessResponse
            {
                Success = true,
                Data = result
            };
        }

        public async Task<ApplicationUserDto> GetUser(string userId)
        {
            ApplicationUser user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            return new ApplicationUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Active = user.Active ? "Active" : "Not Active",
                EmailConfirmed = user.EmailConfirmed ? "Confirmed" : "Not Confirmed",
                LockedOut = user.LockoutEnd?.ToString("dd MMMM yyyy HH:mm:ss"),
                CreatedAt = user.CreatedAt.ToString("dd MMMM yyyy HH:mm:ss"),
            };
        }

        public async Task<SuccessResponse> DeleteUser(string userId)
        {
            ApplicationUser user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            await _userRepo.DeleteAsync(user);
            return new SuccessResponse
            {
                Success = true
            };
        }

        public async Task<SuccessResponse> LockUser(LockUserRequest request)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            DateTimeOffset lockoutEnd = DateTimeOffset.UtcNow.AddHours(request.Duration);
            user.LockoutEnd = lockoutEnd;

            await _userManager.UpdateAsync(user);
            return new SuccessResponse
            {
                Success = true
            };
        }
    }
}
