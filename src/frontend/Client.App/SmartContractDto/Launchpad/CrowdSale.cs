using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto.Launchpad
{
    public class CrowdSale
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long Id { get; set; }
        public string Initiator { get; set; }
        public string Name { get; set; }
        public string TokenSymbol { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long SoftCapNativeTokenAmount { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long HardCapNativeTokenAmount { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long TokenAmountPerNativeToken { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long NativeTokenPurchaseLimitPerBuyerAddress { get; set; }
        public TimestampOutput SaleStartDate { get; set; }
        public TimestampOutput SaleEndDate { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long LockUntilDurationInMinutes { get; set; }
        public bool IsActive { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsSuccess { get; set; }
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long LockId { get; set; }
    }
}


//message CrowdSale
//{
//    int64 id = 1;
//    client.Address initiator = 2;
//    string name = 3;
//    string token_symbol = 4;
//    int64 soft_cap_native_token_amount = 5;
//    int64 hard_cap_native_token_amount = 6;
//    int64 token_amount_per_native_token = 7;
//    int64 native_token_purchase_limit_per_buyer_address = 8;
//    google.protobuf.Timestamp sale_start_date = 9;
//    google.protobuf.Timestamp sale_end_date = 10;
//    int64 lock_until_duration_in_minutes = 11;
//    bool is_active = 12;
//    bool is_cancelled = 13;
//    bool is_success = 14;
//    int64 lock_id = 15;
//}
