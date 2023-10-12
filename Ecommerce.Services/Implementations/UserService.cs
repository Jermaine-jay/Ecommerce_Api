using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Models.Enums;
using Ecommerce.Services.Configurations.Cache.CacheServices;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sprache;
using System.Net;

namespace Ecommerce.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ApplicationUser> _userRepo;
        //private readonly INotificationService _notificationService;
        private readonly IRepository<ProductVariation> _productVariationRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;


        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _cacheService = cacheService;
            _productVariationRepo = _unitOfWork.GetRepository<ProductVariation>();
            _userRepo = _unitOfWork.GetRepository<ApplicationUser>();
        }


        public async Task<SuccessResponse> ChangePassword(string userId, ChangePasswordRequest request)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User Not Found");

            await _userManager.ChangePasswordAsync(user, request.NewPassword, request.CurrentPassword);
            return new SuccessResponse
            {
                Success = true,
            };
        }


        public async Task<SuccessResponse> DeleteAccount(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId)
                ?? throw new InvalidOperationException("User Not Found");

            await _userRepo.DeleteAsync(user);
            return new SuccessResponse
            {
                Success = true
            };
        }


        public async Task<ProfileResponse> Profile(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId)
                ?? throw new InvalidOperationException("User Not Found");

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
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId)
                ?? throw new InvalidOperationException("User Not Found");

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


        public async Task<Cart> GetCart(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId)
                ?? throw new InvalidOperationException("User Not Found");

            var key = $"cart:{user.Id}";
            var cart = await _cacheService.ReadFromCache<Cart>(key)
                ?? throw new InvalidOperationException("User cart Not Found");

            if (cart?.CartItems != null)
            {
                var result = new Cart
                {
                    Id = cart.Id,
                    CartItems = cart.CartItems.OrderByDescending(u => u.CreatedAt).Select(u => new CartItem
                    {
                        Quantity = u.Quantity,
                        UnitPrice = u.UnitPrice,
                        Colour = u.Colour,
                        ProductName = u.ProductName,
                        ProductImage = u.ProductImage,
                    }).ToList(),
                };
                return result;
            }

            return cart;
        }


        public async Task<CartItemResponse> AddToCart(string userId, AddToCartRequest request)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId)
                ?? throw new InvalidOperationException("User cart Not Found");

            var key = $"cart:{user.Id}";
            var cart = await _cacheService.ReadFromCache<Cart>(key);
            //?? throw new InvalidOperationException("cart Not Found");

            var productvar = await _productVariationRepo.GetSingleByAsync(u => u.ProductId.ToString().Equals(request.ProductId), include: img => img.Include(i => i.ProductImages))
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
                Id = Guid.NewGuid(),
                /*ProductImage = productvar.ProductImages.FirstOrDefault().Url,
                ProductName = productvar.Product.Name,*/
                CartId = cart.Id,
                Quantity = request.Quantity,
                Colour = colour,
                //UnitPrice = productvar.Price,
                UnitPrice = 150000,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            if (cart.CartItems == null)
            {
                cart.CartItems = new List<CartItem>();
            }

            cart.CartItems.Add(cartitem);


            await _cacheService.WriteToCache(key, cart, null, TimeSpan.FromDays(365));

            return new CartItemResponse
            {
                Success = true,
                StatusCode = HttpStatusCode.OK,
                Data = cartitem
            };

        }


        public async Task<SuccessResponse> DeleteFromCart(string userId, string cartitemId)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId)
                ?? throw new InvalidOperationException("User cart Not Found");

            var key = $"cart:{user.Id}";
            var cart = await _cacheService.ReadFromCache<Cart>(key)
                ?? throw new InvalidOperationException("User cart Not Found");

            var cartItemToRemove = cart.CartItems.Where(item => item.Id.ToString() == cartitemId).FirstOrDefault()
                ?? throw new InvalidOperationException("Item Not Found");

            if (cartItemToRemove != null)
            {
                cart.CartItems.Remove(cartItemToRemove);
                await _cacheService.WriteToCache(key, cart, null, TimeSpan.FromDays(365));
            }

            return new SuccessResponse
            {
                Success = true,
            };
        }


        public async Task<SuccessResponse> DeleteCartItems(string userId)
        {
            var user = await _userRepo.GetSingleByAsync(user => user.Id.ToString() == userId)
               ?? throw new InvalidOperationException("User cart Not Found");

            var key = $"cart:{user.Id}";
            var cart = await _cacheService.ReadFromCache<Cart>(key)
                ?? throw new InvalidOperationException("User cart Not Found");

            var itemList = cart.CartItems.ToList()
             ?? throw new InvalidOperationException("Items Not Found");


            foreach (var item in itemList)
            {
                cart.CartItems.Remove(item);
            }

            await _cacheService.WriteToCache(key, cart, null, TimeSpan.FromDays(365));
            return new SuccessResponse
            {
                Success = true,
            };
        }
    }
}
