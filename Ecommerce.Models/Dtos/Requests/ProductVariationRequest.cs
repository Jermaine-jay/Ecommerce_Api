using Microsoft.AspNetCore.Http;

namespace Ecommerce.Models.Dtos.Requests
{
    public class ProductVarionRequest
    {
        public string ProductId { get; set; }
        public int Colour { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public List<IFormFile> files { get; set; }

    }

    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProductVariation { get; set; }
        public string Colour { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string ImageSecureUrl { get; set; }
    }

    public class ImageDto
    {
        public string ImageUrl { get; set; }
        public string ImageSecureUrl { get; set; }
    }
}
