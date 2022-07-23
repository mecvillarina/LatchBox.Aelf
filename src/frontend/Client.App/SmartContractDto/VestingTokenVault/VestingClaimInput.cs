using Client.App.Parameters;

namespace Client.App.SmartContractDto.VestingTokenVault
{
    public class VestingClaimInput
    {
        public long VestingId { get; set; }
        public long PeriodId { get; set; }

        public VestingClaimInput(ClaimVestingParameter parameter)
        {
            VestingId = parameter.VestingId;
            PeriodId = parameter.PeriodId;
        }
    }
}
