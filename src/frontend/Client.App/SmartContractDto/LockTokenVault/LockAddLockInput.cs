using Application.Common.Extensions;
using Client.App.Parameters;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

namespace Client.App.SmartContractDto.LockTokenVault
{
    public class LockAddLockInput
    {
        public string TokenSymbol { get; set; }
        public long TotalAmount { get; set; }
        public Timestamp UnlockTime { get; set; }
        public bool IsRevocable { get; set; }
        public List<LockAddLockReceiverInput> Receivers { get; set; }
        public string Remarks { get; set; }

        public LockAddLockInput(AddLockParameter parameter, int tokenDecimals)
        {
            TokenSymbol = parameter.TokenSymbol;
            IsRevocable = parameter.IsRevocable;
            Remarks = parameter.Remarks;

            Receivers = new List<LockAddLockReceiverInput>();
            var unlockTime = DateTime.SpecifyKind(parameter.UnlockDate.Value.Date.AddDays(1).AddMilliseconds(-1), DateTimeKind.Utc);
            var unlockTimestamp = Timestamp.FromDateTime(unlockTime);
            UnlockTime = unlockTimestamp;

            TotalAmount = 0;
            foreach (var receiver in parameter.Receivers)
            {
                var amount = receiver.Amount.ToChainAmount(tokenDecimals);
                Receivers.Add(new LockAddLockReceiverInput() { Receiver = receiver.ReceiverAddress, Amount = amount });
                TotalAmount += amount;
            }
        }

    }
}
