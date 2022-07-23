using Application.Common.Extensions;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.LockTokenVault;
using Client.Infrastructure.Constants;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.App.Models
{
    public class LockTransactionModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public LockOutput Lock { get; private set; }
        public List<LockReceiverOutput> LockReceivers { get; set; }
        public string InitiatorAddressDisplay { get; private set; }
        public string TotalAmountDisplay { get; private set; }
        public string DateStartDisplay { get; private set; }
        public string DateUnlockDisplay { get; private set; }
        public string RevocableDisplay { get; private set; }
        public string StatusDisplay { get; private set; }
        public Color StatusDisplayColor { get; private set; }

        public LockTransactionModel(LockOutput @lock, List<LockReceiverOutput> receivers, TokenInfo tokenInfo)
        {
            Lock = @lock;
            TokenInfo = tokenInfo;
            LockReceivers = receivers;

            var totalAmount = receivers.Sum(x => x.Amount);

            InitiatorAddressDisplay = Lock.Initiator;
            DateStartDisplay = Lock.StartTime.GetUniversalDateTime().LocalDateTime.ToString(ClientConstants.LongDateTimeFormat);
            DateUnlockDisplay = Lock.UnlockTime.GetUniversalDateTime().LocalDateTime.ToString(ClientConstants.LongDateTimeFormat);
            RevocableDisplay = Lock.IsRevocable ? "Yes" : "No";
            TotalAmountDisplay = $"{totalAmount.ToAmount(TokenInfo.Decimals).ToAmountDisplay(TokenInfo.Decimals)} {TokenInfo.Symbol}";

            if (Lock.IsActive)
            {
                if (DateTime.UtcNow < Lock.UnlockTime.GetUniversalDateTime())
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
