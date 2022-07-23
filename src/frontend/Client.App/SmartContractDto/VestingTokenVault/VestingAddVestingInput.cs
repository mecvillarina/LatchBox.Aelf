using Application.Common.Extensions;
using Client.App.Parameters;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingAddVestingInput
    {
        public string TokenSymbol { get; set; }
        public long TotalAmount { get; set; }
        public bool IsRevocable { get; set; }
        public List<VestingAddVestingPeriodInput> Periods { get; set; }

        public VestingAddVestingInput(AddVestingParameter parameter, int tokenDecimals)
        {
            TokenSymbol = parameter.TokenSymbol;
            IsRevocable = parameter.IsRevocable;

            Periods = new List<VestingAddVestingPeriodInput>();

            foreach(var periodParameter in parameter.Periods)
            {
                var period = new VestingAddVestingPeriodInput();
                period.Name = periodParameter.Name;
                period.UnlockTime = Timestamp.FromDateTime(DateTime.SpecifyKind(periodParameter.UnlockDate.Value.Date.AddDays(1).AddMilliseconds(-1), DateTimeKind.Utc));

                period.Receivers = new List<VestingAddVestingReceiverInput>();

                foreach(var receiverParameter in periodParameter.Receivers)
                {
                    var amount = receiverParameter.Amount.ToChainAmount(tokenDecimals);

                    period.Receivers.Add(new VestingAddVestingReceiverInput()
                    {
                        Name = receiverParameter.Name,
                        Address = receiverParameter.ReceiverAddress,
                        Amount = amount
                    });

                    period.TotalAmount += amount;
                }

                Periods.Add(period);

                TotalAmount += period.TotalAmount;
            }
        }
    }
}
