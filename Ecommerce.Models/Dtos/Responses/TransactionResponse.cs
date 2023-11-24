using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Models.Dtos.Responses
{
    public class TransactionResponse
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("reference")]
        public string? Reference { get; set; }

        [JsonProperty("authorization_url")]
        public string? AuthorizationUrl { get; set; }
    }

    public class FlutterTransactionResponse
    {
        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("reference")]
        public string? Reference { get; set; }

        [JsonProperty("authorization_url")]
        public string? AuthorizationUrl { get; set; }
    }
}
