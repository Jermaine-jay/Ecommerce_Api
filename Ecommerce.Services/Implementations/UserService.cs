using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Models.Enums;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Ecommerce.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepo;
        //private readonly INotificationService _notificationService;
        private readonly IRepository<Task> _taskRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly IRepository<ProductVariation> _productVariationRepo;
        private readonly IRepository<Cart> _cartRepo;
        private readonly IRepository<CartItem> _cartItemRepo;
        private readonly IUnitOfWork _unitOfWork;


        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _taskRepo = _unitOfWork.GetRepository<Task>();
            _productRepo = _unitOfWork.GetRepository<Product>();
            _productVariationRepo = _unitOfWork.GetRepository<ProductVariation>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
            _cartRepo = _unitOfWork.GetRepository<Cart>();
            _cartItemRepo = _unitOfWork.GetRepository<CartItem>();
        }


        public async Task<SuccessResponse> ChangePassword(string userId, ChangePasswordRequest request)
        {

            ApplicationUser user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");


            await _userManager.ChangePasswordAsync(user, request.NewPassword, request.CurrentPassword);
            return new SuccessResponse
            {
                Success = true,
            };
        }

        public async Task<SuccessResponse> DeleteAccount(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            await _userRepo.DeleteAsync(user);
            return new SuccessResponse
            {
                Success = true
            };
        }

        public async Task<ProfileResponse> Profile(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            return new ProfileResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
            };
        }

        public async Task<SuccessResponse> UpdateAccount(string userId, UpdateUserRequest request)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId);
            if (user == null)
                throw new InvalidOperationException("User Not Found");

            user.Email = request.Email ?? user.Email;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;

            await _userManager.UpdateAsync(user);
            return new SuccessResponse
            {
                Success = true,
            };
        }

        public async Task<CartResponse> GetCart(string userId)
        {
            var cart = await _cartRepo.GetSingleByAsync(u => u.UserId.ToString().Equals(userId), include: u => u.Include(p => p.CartItems), tracking: true);
            if (cart == null)
                throw new InvalidOperationException("User cart Not Found");


            var result = new CartResponse
            {
                TotalPrice = cart.TotalPrice,
                UserId = cart.UserId,
                Items = cart.CartItems.OrderByDescending(u=> u.CreatedAt).Select(u => new CartItemDto
                {
                    Quantity = u.Quantity,
                    UnitPrice = u.UnitPrice,
                    Colour = u.Colour,
                    Name = u.Product.Name,

                }),
            };

            return result;
        }

        public async Task<CartItemResonse> AddToCart(string userId, AddToCartRequest request)
        {
            var cart = await _cartRepo.GetSingleByAsync(u => u.UserId.ToString().Equals(userId))
                ?? throw new InvalidOperationException("User cart Not Found");

            var productvariation = await _productVariationRepo.GetSingleByAsync(u => u.ProductId.ToString().Equals(request.ProductId))
                ?? throw new InvalidOperationException("Product Not Found");

            var colour = Colour.AsDisplayed;
            switch (request.Colour)
            {
                case (int)Colour.Blue:
                    colour = Colour.Blue;
                    break;

                case (int)Colour.Yellow:
                    colour = Colour.Yellow;
                    break;

                case (int)Colour.Green:
                    colour = Colour.Green;
                    break;

                case (int)Colour.Red:
                    colour = Colour.Green;
                    break;

                case (int)Colour.White:
                    colour = Colour.White;
                    break;

                case (int)Colour.Purple:
                    colour = Colour.Purple;
                    break;
            }

            var cartitem = new CartItem
            {
                ProductId = productvariation.ProductId,
                CartId = cart.Id,
                Quantity = request.Quantity,
                Colour = colour,
                UnitPrice = productvariation.Price,
            };

            cart.TotalPrice = cart.TotalPrice + (cartitem.Quantity * cartitem.UnitPrice);
            cart.UpdatedAt = DateTime.UtcNow;

            await _cartItemRepo.AddAsync(cartitem);
            await _cartRepo.UpdateAsync(cart);

            return new CartItemResonse
            {
                Success = true,
                StatusCode = HttpStatusCode.OK,
                Data = cartitem
            };

        }

        public async Task<SuccessResponse> DeleteFromCart(string userId, string cartitemId)
        {
            var cart = await _cartRepo.GetSingleByAsync(u => u.UserId.ToString().Equals(userId))
                ?? throw new InvalidOperationException("User cart Not Found");

            var cartitem = await _cartItemRepo.GetSingleByAsync(u => u.Id.Equals(cartitemId))
                ?? throw new InvalidOperationException("Item Not Found");

            await _cartItemRepo.DeleteAsync(cartitem);
            return new SuccessResponse
            {
                Success = true,
            };
        }
    }
}
