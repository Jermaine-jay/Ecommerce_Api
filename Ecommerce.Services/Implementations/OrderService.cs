using Ecommerce.Data.Interfaces;
using Ecommerce.Models.Dtos.Requests;
using Ecommerce.Models.Dtos.Responses;
using Ecommerce.Models.Entities;
using Ecommerce.Services.Configurations.Cache.CacheServices;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<ProductVariation> _variationRepo;


        public OrderService(IUnitOfWork unitOfWork, ICacheService cacheService, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _cacheService = cacheService;
            _orderRepo = _unitOfWork.GetRepository<Order>();
            _productRepo = _unitOfWork.GetRepository<Product>();
            _variationRepo = _unitOfWork.GetRepository<ProductVariation>();
        }

        public async Task<SuccessResponse> ClearCart(string userId)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("User does not exist");

            string key = $"cart:{user.Id}";
            Cart cart = await _cacheService.ReadFromCache<Cart>(key)
                ?? throw new InvalidOperationException("User cart not found");


            Order order = new Order
            {
                UserId = user.Id.ToString(),
                UserName = $"{user.FirstName} {user.LastName}",
                Total = cart.CartItems.Sum(u => u.UnitPrice * u.Quantity),
                Received = false,
                Paid = false,
                OrderItems = cart.CartItems.Select(item => new OrderItem
                {
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
            ApplicationUser user = await _userManager.FindByIdAsync(userId)
                 ?? throw new InvalidOperationException("User does not exist");

            ProductVariation variation = await _variationRepo.GetSingleByAsync(pv => pv.Id.Equals(request.VariationId))
                ?? throw new InvalidOperationException("product does not exist");

            OrderItem orderItems = new OrderItem
            {
                ProductName = variation.Product.Name,
                Colour = variation.Colour,
                Quantity = request.Quantity,
                UnitPrice = variation.Price,
            };

            Order order = new Order
            {
                UserName = $"{user.FirstName} {user.LastName}",
                Total = variation.Price * request.Quantity,
                Received = false,
                Paid = false,
                UserId = user.Id.ToString(),
                OrderItems = new List<OrderItem>()
                {
                    orderItems
                }
            };

            order.OrderItems.Add(orderItems);
            _unitOfWork.SaveChanges();

            OrderResponse result = new OrderResponse
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
            Order order = await _orderRepo.GetSingleByAsync(o => o.Id.Equals(request.OrderId))
                ?? throw new InvalidOperationException("order not found");

            ShippingAddress address = new ShippingAddress
            {
                OrderId = order.Id,
                City = request.City,
                Country = request.Country,
                Street = request.Street,
                Postcode = request.PostCode,
                HomeNumber = request.HomeNumber,
            };

            order.ShippingAddress = address;
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
