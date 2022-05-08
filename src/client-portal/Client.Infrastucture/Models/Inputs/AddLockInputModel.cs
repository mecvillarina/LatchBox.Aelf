using System;
using System.Collections.Generic;

namespace Client.Infrastructure.Models.Inputs
{
    public class AddLockInputModel
    {
        public string TokenSymbol { get; set; }
        public long TotalAmount { get; set; }
        public DateTime UnlockTime { get; set; }
        public bool IsRevocable { get; set; }
        public List<AddLockReceiverInputModel> Receivers { get; set; }
    }

    public class AddLockReceiverInputModel
    {
        public string ReceiverAddress { get; set; }
        public long Amount { get; set; }
    }
}
