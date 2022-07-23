using Application.Common.Extensions;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.VestingTokenVault;
using MudBlazor;
using System;

namespace Client.App.Models
{
    public class VestingReceiverModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public VestingOutput Vesting { get; private set; }
        public VestingPeriodOutput Period { get; set; }
        public VestingReceiverOutput Receiver { get; set; }
        public string StatusDisplay { get; private set; }
        public Color StatusColor { get; private set; }

        public bool CanClaim { get; private set; }
        public string AmountDisplay { get; private set; }

        public VestingReceiverModel(VestingGetVestingTransactionForReceiverOutput output)
        {
            Vesting = output.Vesting;
            Period = output.Period;
            Receiver = output.Receiver;

            if (Receiver.DateRevoked != null)
            {
                StatusDisplay = "Revoked";
                StatusColor = Color.Error;
            }
            else if (Receiver.DateClaimed != null)
            {
                StatusDisplay = "Claimed";
                StatusColor = Color.Info;
            }
            else
            {
                if (Vesting.IsActive)
                {
                    if (DateTime.UtcNow < Period.UnlockTime.GetUniversalDateTime())
                    {
                        StatusDisplay = "Locked";
                        StatusColor = Color.Primary;
                    }
                    else
                    {
                        StatusDisplay = "Unlocked";
                        StatusColor = Color.Info;
                        CanClaim = true;
                    }
                }
            }
        }

        public void SetTokenInfo(TokenInfo tokenInfo)
        {
            TokenInfo = tokenInfo;
            AmountDisplay = $"{Receiver.Amount.ToAmount(TokenInfo.Decimals).ToAmountDisplay(TokenInfo.Decimals)} {TokenInfo.Symbol}";
        }
    }
}
