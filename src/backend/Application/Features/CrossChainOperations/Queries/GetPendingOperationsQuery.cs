using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CrossChainOperations.Queries
{
    public class GetPendingOperationsQuery : IRequest<Result<List<CrossChainPendingOperationDto>>>
    {
        public string From { get; set; }
        public int IssueChainId { get; set; }
        public string ContractName { get; set; }

        public class GetPendingOperationsQueryHandler : IRequestHandler<GetPendingOperationsQuery, Result<List<CrossChainPendingOperationDto>>>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IAElfService _aElfService;

            public GetPendingOperationsQueryHandler(IApplicationDbContext dbContext, IAElfService aElfService)
            {
                _dbContext = dbContext;
                _aElfService = aElfService;
            }

            public async Task<Result<List<CrossChainPendingOperationDto>>> Handle(GetPendingOperationsQuery request, CancellationToken cancellationToken)
            {
                var issueChain = _dbContext.ChainInfos.FirstOrDefault(x => x.ChainId == request.IssueChainId);

                if (issueChain == null) return await Result<List<CrossChainPendingOperationDto>>.FailAsync("Issue Chain not supported.");

                var ops = _dbContext.CrossChainOperations.Where(x => x.IssueChainId == request.IssueChainId && x.From == request.From && x.ContractName == request.ContractName && !x.IsConfirmed).ToList();

                var data = new List<CrossChainPendingOperationDto>();

                foreach (var op in ops)
                {
                    var chain = _dbContext.ChainInfos.FirstOrDefault(x => x.ChainId == op.ChainId);
                    if (chain != null && (chain.LastIrreversibleBlockHeight - op.ChainBlockNumber) > 80)
                    {
                        var item = new CrossChainPendingOperationDto();
                        item.FromChainId = op.ChainId;
                        item.IssueChainId = op.IssueChainId;
                        item.Transaction = JsonSerializer.Deserialize<TransactionResultDto>(op.RawTxData);
                        item.MerklePath = JsonSerializer.Deserialize<MerklePathDto>(op.RawMerklePathData);
                        item.IssueChainOperation = op.IssueChainOperation;
                        item.ExplorerUrl = $"{chain.Explorer}/tx/{item.Transaction.TransactionId}";
                        data.Add(item);
                    }
                }

                return await Result<List<CrossChainPendingOperationDto>>.SuccessAsync(data);
            }
        }
    }
}
