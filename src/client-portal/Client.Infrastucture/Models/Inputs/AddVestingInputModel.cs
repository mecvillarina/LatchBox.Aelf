using System;
using System.Collections.Generic;

namespace Client.Infrastructure.Models.Inputs
{
    public class AddVestingInputModel
    {
        public string TokenSymbol { get; set; }
        public long TotalAmount { get; set; }
        public bool IsRevocable { get; set; }
        public List<AddVestingPeriodInputModel> Periods { get; set; }
    }

    public class AddVestingPeriodInputModel
    {
        public string Name { get; set; }
        public long TotalAmount { get; set; }
        public DateTime UnlockTime { get; set; }
        public List<AddVestingReceiverInputModel> Receivers { get; set; }
    }

    public class AddVestingReceiverInputModel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public long Amount { get; set; }
    }
}
