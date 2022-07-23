using System.Numerics;

namespace Client.App.Parameters
{
    public class ClaimVestingParameter
    {
        public string AmountDisplay { get; set; }
        public long VestingId { get; set; }
        public long PeriodId { get; set; }
        public string PeriodName { get; set; }
        public string ReceiverAddress { get; set; }
    }
}
