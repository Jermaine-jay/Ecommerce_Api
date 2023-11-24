using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Models.Dtos.Requests
{
    public class CreateCategoryRequest
    {
        public string? Name { get; set; }
    }

    public class LockUserRequest
    {
        public string UserId { get; set; }
        public int Duration { get; set; }
    }

    public class CreateProductRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? CategoryId { get; set; }
    }

    public class UpdateProductRequest
    {
        public string? Name { get; set; }
        public string? ProductId { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? CategoryName { get; set; } = null;
    }
}
