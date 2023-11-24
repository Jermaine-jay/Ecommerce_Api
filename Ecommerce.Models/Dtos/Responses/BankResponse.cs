using Newtonsoft.Json;

namespace Ecommerce.Models.Dtos.Responses
{
    public class BankResponse
    {
        [JsonProperty("account_number")]
        public string? BankName { get; set; }

        [JsonProperty("account_name")]
        public string? BankCode { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }
}
