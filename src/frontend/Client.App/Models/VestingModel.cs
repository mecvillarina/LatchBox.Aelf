using Application.Common.Extensions;
using Client.App.SmartContractDto;
using Client.App.SmartContractDto.VestingTokenVault;
using Client.Infrastructure.Constants;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.App.Models
{
    public class VestingModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public VestingOutput Vesting { get; private set; }
        public List<VestingTransactionPeriodOutput> Periods { get; private set; }
        public string InitiatorAddressDisplay { get; private set; }
        public string TotalAmountDisplay { get; private set; }
        public string DateCreationDisplay { get; private set; }
        public string RevocableDisplay { get; private set; }
        public string StatusDisplay { get; private set; }
        public Color StatusDisplayColor { get; private set; }

        public VestingModel(VestingGetVestingTransactionOutput output, TokenInfo tokenInfo)
        {
            Vesting = output.Vesting;
            Periods = output.Periods.ToList();
            TokenInfo = tokenInfo;

            InitiatorAddressDisplay = Vesting.Initiator;
            DateCreationDisplay = Vesting.CreationTime.GetUniversalDateTime().LocalDateTime.ToString(ClientConstants.LongDateTimeFormat);
            RevocableDisplay = Vesting.IsRevocable ? "Yes" : "No";
            TotalAmountDisplay = $"{Vesting.TotalAmount.ToAmount(TokenInfo.Decimals).ToAmountDisplay(TokenInfo.Decimals)} {TokenInfo.Symbol}";

            if (Vesting.IsActive)
            {
                if (Periods.Any(x => DateTime.UtcNow < x.Period.UnlockTime.GetUniversalDateTime()))
                {
                    StatusDisplay = "On Vesting";
                    StatusDisplayColor = Color.Primary;
                }
                else
                {
                    StatusDisplay = "Unlocked";
                    StatusDisplayColor = Color.Info;
                }
            }
            else if (Vesting.IsRevoked)
            {
                StatusDisplay = "Revoked";
                StatusDisplayColor = Color.Error;
            }
            else
            {
                StatusDisplay = "Completed";
                StatusDisplayColor = Color.Info;
            }

        }
    }
}
