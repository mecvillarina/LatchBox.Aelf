using Application.Common.Extensions;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.VestingTokenVault;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.App.Models
{
    public class VestingByInitiatorModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public VestingOutput Vesting { get; private set; }
        public List<VestingPeriodOutput> Periods { get; private set; }
        public long TotalAmount { get; private set; }
        public string TotalAmountDisplay { get; private set; }
        public string PeriodDisplay { get; set; }
        public string StatusDisplay { get; private set; }
        public Color StatusColor { get; private set; }
        public bool IsRevocable { get; private set; }
        public VestingPeriodOutput UpcomingPeriod { get; private set; }

        public VestingByInitiatorModel(VestingGetVestingOutput transaction)
        {
            Vesting = transaction.Vesting;
            Periods = transaction.Periods.ToList();

            TotalAmount = Vesting.TotalAmount;

            if (Vesting.IsActive)
            {
                if (Periods.Any(x => DateTime.UtcNow < x.UnlockTime.GetUniversalDateTime()))
                {
                    StatusDisplay = "On Vesting";
                    StatusColor = Color.Primary;
                }
                else
                {
                    StatusDisplay = "Unlocked";
                    StatusColor = Color.Info;
                }
            }
            else if (Vesting.IsRevoked)
            {
                StatusDisplay = "Revoked";
                StatusColor = Color.Error;
            }
            else
            {
                StatusDisplay = "Completed";
                StatusColor = Color.Info;
            }

            IsRevocable = Vesting.IsRevocable;
            PeriodDisplay = $"{transaction.Periods.Count}";
            UpcomingPeriod = transaction.Periods.FirstOrDefault(x => x.UnlockTime.GetUniversalDateTime() > DateTime.UtcNow);
        }

        public void SetTokenInfo(TokenInfo tokenInfo)
        {
            TokenInfo = tokenInfo;
            TotalAmountDisplay = $"{TotalAmount.ToAmount(TokenInfo.Decimals).ToAmountDisplay(TokenInfo.Decimals)} {TokenInfo.Symbol}";
        }
    }
}
