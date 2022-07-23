using Application.Common.Extensions;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.LockTokenVault;
using Client.Infrastructure.Extensions;
using MudBlazor;
using System;

namespace Client.App.Models
{
    public class LockForReceiverModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public LockOutput Lock { get; private set; }
        public LockReceiverOutput Receiver { get; set; }
        public string Status { get; private set; }
        public Color StatusColor { get; private set; }

        public bool CanClaim { get; private set; }
        public string AmountDisplay { get; private set; }

        public LockForReceiverModel(LockOutput @lock, LockReceiverOutput receiver)
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
                    if (DateTime.UtcNow < Lock.UnlockTime.GetUniversalDateTime())
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
