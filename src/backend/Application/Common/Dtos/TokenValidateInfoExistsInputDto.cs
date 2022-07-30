using System.Text.Json.Serialization;

namespace Application.Common.Dtos
{
    public class TokenValidateInfoExistsInputDto
    {
        public string Symbol { get; set; }
        public string TokenName { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long TotalSupply { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int Decimals { get; set; }
        public string Issuer { get; set; }
        public bool IsBurnable { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int IssueChainId { get; set; }
    }
}
