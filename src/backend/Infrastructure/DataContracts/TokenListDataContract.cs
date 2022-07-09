using System.Text.Json.Serialization;

namespace Infrastructure.DataContracts
{
    public class TokenListDataContract
    {
        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("code")]
        public long Code { get; set; }

        [JsonPropertyName("data")]
        public TokenListDataDataContract Data { get; set; }
    }
}
