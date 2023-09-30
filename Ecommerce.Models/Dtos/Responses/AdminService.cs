using System.Net;

namespace Ecommerce.Models.Dtos.Responses
{
    public class CreateCategoryResponse
    {
        public string Message { get; set; }
        public HttpStatusCode Status { get; set; }
        public object Data { get; set; }
    }

    public class CreateProductResponse
    {
        public string Message { get; set; }
        public HttpStatusCode Status { get; set; }
        public bool IsSuccess { get; set; }
        public object Data { get; set; }
    }
}
