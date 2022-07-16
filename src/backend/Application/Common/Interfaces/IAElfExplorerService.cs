using Application.Common.Dtos;
using System.Collections.Generic;

namespace Application.Common.Interfaces
{
    public interface IAElfExplorerService
    {
        List<TokenDto> GetTokenList(string explorerUrl);
        List<TokenBalanceInfoDto> GetTokenBalanceList(string explorerUrl, string address);
        TransactionResultDto GetTx(string explorerUrl, string txId);
    }
}
