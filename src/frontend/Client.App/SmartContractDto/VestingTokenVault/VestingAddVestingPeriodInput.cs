using Google.Protobuf.WellKnownTypes;
using System.Collections.Generic;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingAddVestingPeriodInput
    {
        public string Name { get; set; }
        public long TotalAmount { get; set; }
        public Timestamp UnlockTime { get; set; }
        public List<VestingAddVestingReceiverInput> Receivers { get; set; } = new();
    }
}
