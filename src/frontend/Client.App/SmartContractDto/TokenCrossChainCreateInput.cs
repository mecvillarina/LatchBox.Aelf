using Application.Common.Dtos;
using Google.Protobuf;

namespace Client.App.SmartContractDto
{
    public class TokenCrossChainCreateInput
    {
        public int FromChainId { get; set; }
        public long ParentChainHeight { get; set; }
        public string TransactionBytes { get; set; }
        public MerklePathDto MerklePath { get; set; }
    }
}
