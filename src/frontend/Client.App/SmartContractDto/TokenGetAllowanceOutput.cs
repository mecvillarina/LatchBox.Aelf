using AElf.Types;
using MediatR;
using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto
{
    public class TokenGetAllowanceOutput
    {
        public string Symbol { get; set; }
        public string Owner { get; set; }
        public string Spender { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Allowance { get; set; }
    }
}
