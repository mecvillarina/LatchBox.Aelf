using Application.Common.Extensions;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.LockTokenVault;
using MudBlazor;
using System;

namespace Client.App.Models
{
    public class LockModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public LockOutput Lock { get; private set; }
        public string TotalAmountDisplay { get; private set; }

        public string Status { get; private set; }
        public Color StatusColor { get; private set; }

        public bool IsRevocable { get; private set; }

        public LockModel(LockOutput @lock)
        {
            Lock = @lock;

            if (Lock.IsActive)
            {
                if (DateTime.UtcNow < Lock.UnlockTime.GetUniversalDateTime())
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
