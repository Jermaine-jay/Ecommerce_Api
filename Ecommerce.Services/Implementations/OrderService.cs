using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Services.Configurations.Cache.CacheServices;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Ecommerce.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<ProductVariation> _variationRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly ICacheService _cacheService;
        private readonly UserManager<ApplicationUser> _userManager;


        public OrderService(IUnitOfWork unitOfWork, ICacheService cacheService, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _userManager = userManager;
            _orderRepo = _unitOfWork.GetRepository<Order>();
            _variationRepo = _unitOfWork.GetRepository<ProductVariation>();
            _productRepo = _unitOfWork.GetRepository<Product>();
        }


        public async Task<SuccessResponse> ClearCart(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User does not exist");

            var key = $"cart:{user.Id}";
            var cart = await _cacheService.ReadFromCache<Cart>(key)
                ?? throw new InvalidOperationException("User cart not found");


            var order = new Order
            {
                ApplicationUserId = user.Id,
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                UserName = $"{user.FirstName} {user.LastName}",
                Total = cart.CartItems.Sum(u => u.UnitPrice * u.Quantity),
                Received = false,
                Paid = false,
                OrderItems = cart.CartItems.Select(item => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductName = item.ProductName ?? "Unknown Product",
                    Colour = item.Colour,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                }).ToList()
            };

            await _orderRepo.AddAsync(order);
            foreach (var item in cart.CartItems.ToList())
            {
                cart.CartItems.Remove(item);
            }

            await _cacheService.WriteToCache(key, cart, null, TimeSpan.FromDays(365));
            return new SuccessResponse
            {
                Success = true,
                Data = order
            };
        }

        public async Task<OrderResponse> CreateOrder(string userId, OrderRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId)
                 ?? throw new InvalidOperationException("User does not exist");

            var variation = await _variationRepo.GetSingleByAsync(pv => pv.Id.Equals(request.VariationId))
                ?? throw new InvalidOperationException("product does not exist");

            var id = Guid.NewGuid();
            var orderItems = new OrderItem
            {
                ProductName = variation.Product.Name,
                Colour = variation.Colour,
                Quantity = request.Quantity,
                UnitPrice = variation.Price,
                OrderId = id,
            };

            var order = new Order
            {
                Id = id,
                CreatedAt = DateTime.UtcNow,
                UserName = $"{user.FirstName} {user.LastName}",
                Total = variation.Price * request.Quantity,
                Received = false,
                Paid = false,
                ApplicationUserId = user.Id,
                OrderItems = new List<OrderItem>()
                {
                    orderItems
                }
            };

            order.OrderItems.Add(orderItems);
            _unitOfWork.SaveChanges();

            var result = new OrderResponse
            {
                Id = order.Id.ToString(),
                OrderDate = order.CreatedAt.ToString("dd MMMM yyyy HH:mm:ss"),
                UserName = order.UserName,
                Total = order.Total,
                Received = order.Received ? "Received" : "Not yet Received",
                Paid = order.Paid ? "Paid" : "No Payment Made",
            };

            return result;
        }

        public async Task<OrderResponse> ShippingAddress(ShippingAddressRequest request)
        {
            var order = await _orderRepo.GetSingleByAsync(o => o.Id.Equals(request.OrderId))
                ?? throw new InvalidOperationException("order not found");

            var address = new ShippingAddress
            {
                OrderId = order.Id,
                City = request.City,
                Country = request.Country,
                Street = request.Street,
                Postcode = request.PostCode,
                HomeNumber = request.HomeNumber,
            };

            order.ShippingAddress = address;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepo.UpdateAsync(order);

            return new OrderResponse
            {
                Id = order.Id.ToString(),
                OrderDate = order.CreatedAt.ToString("dd MMMM yyyy HH:mm:ss"),
                UserName = order.UserName,
                Total = order.Total,
                Received = order.Received ? "Received" : "Not yet Received",
                Paid = order.Paid ? "Paid" : "No Payment Made",
                HomeNumber = address.HomeNumber,
                City = address.City,
                Country = address.Country,
                Street = address.Street,
                Postcode = address.Postcode,
            };
        }
    }
}
