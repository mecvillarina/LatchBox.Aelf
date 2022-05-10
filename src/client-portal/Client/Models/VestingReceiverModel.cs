using AElf.Client.LatchBox.VestingTokenVault;
using AElf.Client.MultiToken;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;
using MudBlazor;

namespace Client.Models
{
    public class VestingReceiverModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public Vesting Vesting { get; private set; }
        public VestingPeriod Period { get; set; }
        public VestingReceiver Receiver { get; set; }
        public string StatusDisplay { get; private set; }
        public Color StatusColor { get; private set; }

        public bool CanClaim { get; private set; }
        public string AmountDisplay { get; private set; }

        public VestingReceiverModel(GetVestingTransactionForReceiverOutput output)
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
                    if (DateTime.UtcNow < Period.UnlockTime.ToDateTime())
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
