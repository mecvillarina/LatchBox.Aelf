using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto.Launchpad
{
    public class CrowdSaleInvestment
    {
        public string Investor { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long TokenAmount { get; set; }
        public TimestampOutput DateLastPurchased { get; set; }
        public TimestampOutput DateRefunded { get; set; }
    }
}

//message CrowdSaleInvestment
//{
//    client.Address investor = 1;
//    int64 token_amount = 2;
//    google.protobuf.Timestamp date_last_purchased = 3;
//    google.protobuf.Timestamp date_refunded = 4;
//}
