namespace Ecommerce.Models.Dtos.Requests
{
    public class UpdateUserRequest
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class AddToCartRequest
    {
        public string? ProductId { get; set; }
        public int Quantity { get; set; }
        public int Colour { get; set; }
    }
}
