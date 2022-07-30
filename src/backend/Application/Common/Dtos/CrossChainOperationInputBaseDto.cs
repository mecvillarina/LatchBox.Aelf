using System.Text.Json.Serialization;

namespace Application.Common.Dtos
{
    public class CrossChainOperationInputBaseDto
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int FromChainId { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long ParentChainHeight { get; set; }
    }
}
