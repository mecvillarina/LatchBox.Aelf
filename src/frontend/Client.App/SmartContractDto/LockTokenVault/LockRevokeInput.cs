using Client.App.Parameters;

namespace Client.App.SmartContractDto.LockTokenVault
{
    public class LockRevokeInput
    {
        public long LockId { get; set; }

        public LockRevokeInput(RevokeLockParameter parameter)
        {
            LockId = parameter.LockId;
        }
    }
}
