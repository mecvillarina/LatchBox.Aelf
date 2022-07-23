namespace Client.App.SmartContractDto
{
    public class TokenApproveInput
    {
        public string Spender { get; set; }
        public string Symbol { get; set; }
        public long Amount { get; set; }
    }
}
