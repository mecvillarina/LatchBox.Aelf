using System.Numerics;

namespace Client.Parameters
{
    public class ClaimVestingParameter
    {
        public string AmountDisplay { get; set; }
        public ulong VestingIdx { get; set; }
        public ulong PeriodIdx { get; set; }
        public string PeriodName { get; set; }
        public string ReceiverAddress { get; set; }
    }
}
