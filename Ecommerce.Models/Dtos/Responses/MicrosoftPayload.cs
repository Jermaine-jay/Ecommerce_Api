using Newtonsoft.Json;

namespace Ecommerce.Models.Dtos.Responses
{

    public class MicrosoftPayload
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("nonce")]
        public string Nonce { get; set; }

        [JsonProperty("oid")]
        public string ObjectId { get; set; }

        [JsonProperty("tid")]
        public string TenantId { get; set; }

        [JsonProperty("aio")]
        public string AIO { get; set; }
    }
}
