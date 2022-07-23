using Client.App.Parameters;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingRevokeInput
    {
        public long VestingId { get; set; }

        public VestingRevokeInput(RevokeVestingParameter parameter)
        {
            VestingId = parameter.VestingId;
        }
    }
}
