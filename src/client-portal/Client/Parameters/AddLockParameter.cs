namespace Client.Parameters
{
    public class AddLockParameter
    {
        public string TokenSymbol { get; set; }
        public DateTime? UnlockDate { get; set; }
        public bool IsRevocable { get; set; }
        public List<AddLockReceiverParameter> Receivers { get; set; } = new List<AddLockReceiverParameter>();
    }

}
