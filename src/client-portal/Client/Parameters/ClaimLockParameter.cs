using System.Numerics;

namespace Client.Parameters
{
    public class ClaimLockParameter
    {
        public string AmountDisplay { get; set; }
        public ulong LockIndex { get; set; }
        public string ReceiverAddress { get; set; }
    }
}
