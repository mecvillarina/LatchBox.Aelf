using AElf.Client.LatchBox.LockTokenVault;
using AElf.Client.MultiToken;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;
using MudBlazor;

namespace Client.Models
{
    public class LockForReceiverModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public Lock Lock { get; private set; }
        public LockReceiver Receiver { get; set; }
        public string Status { get; private set; }
        public Color StatusColor { get; private set; }

        public bool CanClaim { get; private set; }
        public string AmountDisplay { get; private set; }

        public LockForReceiverModel(Lock @lock, LockReceiver receiver)
        {
            Lock = @lock;

            Receiver = receiver;
            if (Receiver.DateRevoked != null)
            {
                Status = "Revoked";
                StatusColor = Color.Error;
            }
            else if (Receiver.DateClaimed != null)
            {
                Status = "Claimed";
                StatusColor = Color.Info;
            }
            else
            {
                if (Lock.IsActive)
                {
                    if (DateTime.UtcNow < Lock.UnlockTime.ToDateTime())
                    {
                        Status = "Locked";
                        StatusColor = Color.Primary;
                    }
                    else
                    {
                        Status = "Unlocked";
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
