using Newtonsoft.Json;

namespace Ecommerce.Models.Dtos.Requests
{
    public class BankPaymentRequest
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("account_number")]
        public string AccountNumber { get; set; }

        [JsonProperty("birthday")]
        public string Birthday { get; set; }
        public string OrderId { get; set; }
    }

    public class FlutterPaymentRequest
    {
        public string OrderId { get; set; }
        public int Currency { get; set; }
    }
}
