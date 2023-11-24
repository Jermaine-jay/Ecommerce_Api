using Ecommerce.Models.Entities;
using Ecommerce.Models.Enums;
using System.Net;

namespace Ecommerce.Models.Dtos.Responses
{
    public class CartResponse
    {
        public decimal? TotalPrice { get; set; }
        public Guid? UserId { get; set; }
        public IEnumerable<CartItemDto>? Items { get; set; }

    }

    public class CartItemDto
    {
        public string? Name { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public Colour Colour { get; set; }

    }

    public class CartItemResponse
    {
        public bool? Success { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
        public CartItem? Data { get; set; }
    }

    public class ProfileResponse
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
