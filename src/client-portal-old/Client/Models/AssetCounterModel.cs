using AElf.Client.LatchBox.LockTokenVault;
using AElf.Client.MultiToken;

namespace Client.Models
{
    public class AssetCounterModel
    {
        public string TokenSymbol { get; set; }
        public long LockedAmount { get; set; }
        public long UnlockedAmount { get; set; }
        public TokenInfo TokenInfo { get; set; }

        public AssetCounterModel(GetLockAssetCounterOutput output)
        {
            TokenSymbol = output.TokenSymbol;
            LockedAmount = output.LockedAmount;
            UnlockedAmount = output.UnlockedAmount;
        }

        public void SetTokenInfo(TokenInfo tokenInfo)
        {
            TokenInfo = tokenInfo;
        }
    }
}
