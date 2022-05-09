using AElf.Client.LatchBox.LockTokenVault;
using AElf.Client.MultiToken;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;
using MudBlazor;
using System.Numerics;

namespace Client.Models
{
    public class LockByInitiatorModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public Lock Lock { get; private set; }
        public string TotalAmountDisplay { get; private set; }

        public string Status { get; private set; }
        public Color StatusColor { get; private set; }

        public bool IsRevocable { get; private set; }

        public LockByInitiatorModel(Lock @lock)
        {
            Lock = @lock;

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
                }
            }
            else if (Lock.IsRevoked)
            {
                Status = "Revoked";
                StatusColor = Color.Error;
            }
            else
            {
                Status = "Claimed";
                StatusColor = Color.Info;
            }

            IsRevocable = Lock.IsRevocable;
        }

        public void SetTokenInfo(TokenInfo tokenInfo)
        {
            TokenInfo = tokenInfo;
            TotalAmountDisplay = $"{Lock.TotalAmount.ToAmountDisplay(tokenInfo.Decimals)} {tokenInfo.Symbol}";
        }
    }
}
