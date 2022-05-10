using AElf.Client.LatchBox.VestingTokenVault;
using AElf.Client.MultiToken;
using Client.Infrastructure.Constants;
using Client.Infrastructure.Extensions;
using MudBlazor;

namespace Client.Models
{
    public class VestingModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public Vesting Vesting { get; private set; }
        public List<VestingTransactionPeriodOutput> Periods { get; private set; }
        public string InitiatorAddressDisplay { get; private set; }
        public string TotalAmountDisplay { get; private set; }
        public string DateCreationDisplay { get; private set; }
        public string RevocableDisplay { get; private set; }
        public string StatusDisplay { get; private set; }
        public Color StatusDisplayColor { get; private set; }

        public VestingModel(GetVestingTransactionOutput output, TokenInfo tokenInfo)
        {
            Vesting = output.Vesting;
            Periods = output.Periods.ToList();
            TokenInfo = tokenInfo;

            InitiatorAddressDisplay = Vesting.Initiator.ToStringAddress();
            DateCreationDisplay = Vesting.CreationTime.ToDateTime().ToString(ClientConstants.LongDateTimeFormat);
            RevocableDisplay = Vesting.IsRevocable ? "Yes" : "No";
            TotalAmountDisplay = $"{Vesting.TotalAmount.ToAmount(TokenInfo.Decimals).ToAmountDisplay(TokenInfo.Decimals)} {TokenInfo.Symbol}";

            if (Vesting.IsActive)
            {
                if (Periods.Any(x => DateTime.UtcNow < x.Period.UnlockTime.ToDateTime()))
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
