using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto
{
    public class GetBalanceOutput
    {
        public string Symbol { get; set; }
        public string Owner { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Balance { get; set; }
    }
}
