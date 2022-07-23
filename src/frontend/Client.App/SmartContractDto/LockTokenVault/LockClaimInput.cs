using Client.App.Parameters;

namespace Client.App.SmartContractDto.LockTokenVault
{
    public class LockClaimInput
    {
        public long LockId { get; set; }

        public LockClaimInput(ClaimLockParameter parameter)
        {
            LockId = parameter.LockId;
        }
    }
}
