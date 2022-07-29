using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CrossChainOperations.Commands.Create
{
    public class CreateCrossChainOperationCommand : IRequest<IResult>
    {
        public string From { get; set; }
        public string ContactName { get; set; }
        public string TransactionId { get; set; }
        public string ChainOperation { get; set; }
        public int ChainId { get; set; }
        public int IssueChainId { get; set; }

        public class CreateCrossChainOperationCommandHandler : IRequestHandler<CreateCrossChainOperationCommand, IResult>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IAElfService _aElfService;
            private readonly IDateTime _dateTime;

            public CreateCrossChainOperationCommandHandler(IApplicationDbContext dbContext, IAElfService aElfService, IDateTime dateTime)
            {
                _dbContext = dbContext;
                _aElfService = aElfService;
                _dateTime = dateTime;
            }

            public async Task<IResult> Handle(CreateCrossChainOperationCommand request, CancellationToken cancellationToken)
            {
                var chain = _dbContext.ChainInfos.FirstOrDefault(x => x.ChainId == request.ChainId);

                if (chain == null) return Result.Fail("Chain not supported.");

                var supportedOperations = _aElfService.GetSupportedCrossChainOperations();
                var supportedOperation = supportedOperations.FirstOrDefault(x => x.ContractName == request.ContactName && x.ChainOperation == request.ChainOperation);

                if (supportedOperation == null) return Result.Fail("Cross Operation Chain not supported.");

                var transaction = _aElfService.GetTransactionByTransactionId(chain.RpcApi, request.TransactionId);

                if (transaction == null) return Result.Fail("Transaction not found.");

                if (transaction.ErrorMessage != null) return Result.Fail(transaction.ErrorMessage.Message);

                var merklePath = _aElfService.GetMainChainMerklePathByTransactionId(chain.RpcApi, request.TransactionId);

                if (merklePath == null) return Result.Fail("Merkle Path not found.");

                var isExits = await _dbContext.CrossChainOperations.AnyAsync(x => x.TransactionId == request.TransactionId);

                if (isExits) return Result.Fail("Transaction already added.");

                var op = new CrossChainOperation()
                {
                    From = request.From,
                    ContractName = supportedOperation.ContractName,
                    ChainOperation = request.ChainOperation,
                    Created = _dateTime.UtcNow,
                    ChainBlockHash = transaction.BlockHash,
                    ChainBlockNumber = transaction.BlockNumber,
                    ChainId = request.ChainId,
                    IssueChainId = request.IssueChainId,
                    IssueChainOperation = supportedOperation.IssueChainOperation,
                    RawTxData = JsonSerializer.Serialize(transaction),
                    RawMerklePathData = JsonSerializer.Serialize(merklePath),
                    TransactionId = request.TransactionId,
                };

                _dbContext.CrossChainOperations.Add(op);
                await _dbContext.SaveChangesAsync();

                return Result.Success();
            }
        }
    }
}
