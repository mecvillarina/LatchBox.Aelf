using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto.Launchpad
{
    public class CrowdSaleOutput
    {
        public CrowdSale CrowdSale { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long RaisedAmount { get; set; }
        public string TokenName { get; set; }
        public string TokenSymbol { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int TokenDecimals { get; set; }
    }
}
//message CrowdSaleOutput
//{
//    CrowdSale crowd_sale = 1;
//    int64 raised_amount = 2;
//    string token_name = 3;
//    string token_symbol = 4;
//    int32 token_decimals = 5;
//}