using Google.Protobuf.WellKnownTypes;
using System;
using System.Text.Json.Serialization;

namespace Client.App.SmartContractDto.LockTokenVault
{
    public class LockOutput
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long LockId { get; set; }
        public string TokenSymbol { get; set; }
        public string Initiator { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public long TotalAmount { get; set; }

        public TimestampOutput CreationTime { get; set; }
        public object StartTime { get; set; }
        public object UnlockTime { get; set; }
        public bool IsRevocable { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsActive { get; set; }
        public string Remarks { get; set; }
    }
}
