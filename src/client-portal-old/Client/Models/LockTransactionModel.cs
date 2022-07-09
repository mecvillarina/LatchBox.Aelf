using AElf.Client.LatchBox.LockTokenVault;
using AElf.Client.MultiToken;
using Client.Infrastructure.Constants;
using Client.Infrastructure.Extensions;
using Client.Infrastructure.Models;
using MudBlazor;
using System.Numerics;

namespace Client.Models
{
    public class LockTransactionModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public Lock Lock { get; private set; }
        public List<LockReceiver> LockReceivers { get; set; }
        public string InitiatorAddressDisplay { get; private set; }
        public string TotalAmountDisplay { get; private set; }
        public string DateStartDisplay { get; private set; }
        public string DateUnlockDisplay { get; private set; }
        public string RevocableDisplay { get; private set; }
        public string StatusDisplay { get; private set; }
        public Color StatusDisplayColor { get; private set; }

        public LockTransactionModel(Lock @lock, List<LockReceiver> receivers, TokenInfo tokenInfo)
        {
            Lock = @lock;
            TokenInfo = tokenInfo;
            LockReceivers = receivers;

            var totalAmount = receivers.Sum(x => x.Amount);

            InitiatorAddressDisplay = Lock.Initiator.ToStringAddress();
            DateStartDisplay = Lock.StartTime.ToDateTime().ToString(ClientConstants.LongDateTimeFormat);
            DateUnlockDisplay = Lock.UnlockTime.ToDateTime().ToString(ClientConstants.LongDateTimeFormat);
            RevocableDisplay = Lock.IsRevocable ? "Yes" : "No";
            TotalAmountDisplay = $"{totalAmount.ToAmount(TokenInfo.Decimals).ToAmountDisplay(TokenInfo.Decimals)} {TokenInfo.Symbol}";

            if (Lock.IsActive)
            {
                if (DateTime.UtcNow < Lock.UnlockTime.ToDateTime())
                {
                    StatusDisplay = "Locked";
                    StatusDisplayColor = Color.Primary;
                }
                else
                {
                    StatusDisplay = "Unlocked";
                    StatusDisplayColor = Color.Info;
                }
            }
            else if (Lock.IsRevoked)
            {
                StatusDisplay = "Revoked";
                StatusDisplayColor = Color.Error;
            }
            else
            {
                StatusDisplay = "Claimed";
                StatusDisplayColor = Color.Info;
            }
        }
    }
}
