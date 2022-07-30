using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CrossChainOperations.Commands.Confirm
{
    public class ConfirmCrossChainOperationCommand : IRequest<IResult>
    {
        public int ChainId { get; set; }
        public string IssueChainTransactionId { get; set; }

        public class ConfirmCrossChainOperationCommandHandler : IRequestHandler<ConfirmCrossChainOperationCommand, IResult>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IAElfService _aElfService;
            private readonly IDateTime _dateTime;

            public ConfirmCrossChainOperationCommandHandler(IApplicationDbContext dbContext, IAElfService aElfService, IDateTime dateTime)
            {
                _dbContext = dbContext;
                _aElfService = aElfService;
                _dateTime = dateTime;
            }

            public async Task<IResult> Handle(ConfirmCrossChainOperationCommand request, CancellationToken cancellationToken)
            {
                var chain = _dbContext.ChainInfos.FirstOrDefault(x => x.ChainId == request.ChainId);

                if (chain == null) return Result.Fail("Chain not supported.");

                var transaction = _aElfService.GetTransactionByTransactionId(chain.RpcApi, request.IssueChainTransactionId);

                if (transaction == null) return Result.Fail("Transaction not found.");

                if (transaction.ErrorMessage != null) return Result.Fail(transaction.ErrorMessage.Message);

                var @params = JsonSerializer.Deserialize<CrossChainOperationInputBaseDto>(transaction.Transaction.Params, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

                var op = _dbContext.CrossChainOperations.FirstOrDefault(x => x.ChainId == @params.FromChainId && x.ChainBlockNumber == @params.ParentChainHeight && !x.IsConfirmed);

                if (op == null) return Result.Fail("Invalid transaction");

                op.IsConfirmed = true;
                op.IssueChainTransactionId = request.IssueChainTransactionId;

                _dbContext.CrossChainOperations.Update(op);
                await _dbContext.SaveChangesAsync();

                return Result.Success();
            }
        }
    }
}
