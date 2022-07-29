namespace Domain.Entities
{
    public class CrossChainOperation
    {
        public int Id { get; set; }
        public string From { get; set; }
        public string TransactionId { get; set; }
        public string ChainOperation { get; set; }
        public long ChainBlockNumber { get; set; }
        public string ChainBlockHash { get; set; }
        public int ChainId { get; set; }
        public int IssueChainId { get; set; }
        public string IssueChainOperation { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
