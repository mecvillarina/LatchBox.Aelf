using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.DataContracts
{
    public class TokenListDataDataContract
    {
        [JsonPropertyName("list")]
        public List<TokenDataContract> Tokens { get; set; }

        [JsonPropertyName("total")]
        public long Total { get; set; }
    }
}
