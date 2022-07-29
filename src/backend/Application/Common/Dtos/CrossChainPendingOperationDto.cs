namespace Application.Common.Dtos
{
    public class CrossChainPendingOperationDto
    {
        public int FromChainId { get; set; }
        public int IssueChainId { get; set; }
        public TransactionResultDto Transaction { get; set; }
        public MerklePathDto MerklePath { get; set; }
        public string IssueChainOperation { get; set; }
    }
}
