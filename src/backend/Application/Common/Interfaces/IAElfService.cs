using Application.Common.Dtos;
using Application.Common.Models;
using System.Collections.Generic;

namespace Application.Common.Interfaces
{
    public interface IAElfService
    {
        ChainStatusDto GetChainStatus(string apiUrl);
        TransactionResultDto GetTransactionByTransactionId(string apiUrl, string transactionId);
        MerklePathDto GetMainChainMerklePathByTransactionId(string apiUrl, string transactionId);
        List<CrossChainOperationModel> GetSupportedCrossChainOperations();
    }
}
