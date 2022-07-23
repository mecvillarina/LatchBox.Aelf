using System.Numerics;

namespace Client.App.Parameters
{
    public class ClaimLockParameter
    {
        public string AmountDisplay { get; set; }
        public long LockId { get; set; }
        public string ReceiverAddress { get; set; }
    }
}
