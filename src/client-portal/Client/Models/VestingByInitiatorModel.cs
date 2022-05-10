using AElf.Client.LatchBox.VestingTokenVault;
using AElf.Client.MultiToken;
using Client.Infrastructure.Extensions;
using MudBlazor;
using System.Numerics;

namespace Client.Models
{
    public class VestingByInitiatorModel
    {
        public TokenInfo TokenInfo { get; private set; }
        public Vesting Vesting { get; private set; }
        public List<VestingPeriod> Periods { get; private set; }
        public long TotalAmount { get; private set; }
        public string TotalAmountDisplay { get; private set; }
        public string PeriodDisplay { get; set; }
        public string StatusDisplay { get; private set; }
        public Color StatusColor { get; private set; }
        public bool IsRevocable { get; private set; }
        public VestingPeriod UpcomingPeriod { get; private set; }

        public VestingByInitiatorModel(GetVestingOutput transaction)
        {
            Vesting = transaction.Vesting;
            Periods = transaction.Periods.ToList();

            TotalAmount = Vesting.TotalAmount;

            if (Vesting.IsActive)
            {
                if (Periods.Any(x => DateTime.UtcNow < x.UnlockTime.ToDateTime()))
                {
                    StatusDisplay = "On Vesting";
                    StatusColor = Color.Primary;
                }
                else
                {
                    StatusDisplay = "Unlocked";
                    StatusColor = Color.Info;
                }
            }
            else if (Vesting.IsRevoked)
            {
                StatusDisplay = "Revoked";
                StatusColor = Color.Error;
            }
            else
            {
                StatusDisplay = "Completed";
                StatusColor = Color.Info;
            }

            IsRevocable = Vesting.IsRevocable;
            PeriodDisplay = $"{transaction.Periods.Count} Periods";
            UpcomingPeriod = transaction.Periods.FirstOrDefault(x => x.UnlockTime.ToDateTime() > DateTime.UtcNow);
        }

        public void SetTokenInfo(TokenInfo tokenInfo)
        {
            TokenInfo = tokenInfo;
            TotalAmountDisplay = $"{TotalAmount.ToAmount(TokenInfo.Decimals).ToAmountDisplay(TokenInfo.Decimals)} {TokenInfo.Symbol}";
        }
    }
}
