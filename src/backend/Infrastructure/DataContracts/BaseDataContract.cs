using System.Text.Json.Serialization;

namespace Infrastructure.DataContracts
{
    public class BaseDataContract<T>
    {
        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("code")]
        public long Code { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }
    }
}
