using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.CrossChainOperations.Commands.Cleanup
{
    public class CleanupCrossChainOperationCommand : IRequest<IResult>
    {
        public class CleanupCrossChainOperationCommandHandler : IRequestHandler<CleanupCrossChainOperationCommand, IResult>
        {
            private readonly IApplicationDbContext _dbContext;

            public CleanupCrossChainOperationCommandHandler(IApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<IResult> Handle(CleanupCrossChainOperationCommand request, CancellationToken cancellationToken)
            {
                var ops = _dbContext.CrossChainOperations.Where(x => x.IsConfirmed).ToList();

                if (ops.Any())
                {
                    _dbContext.CrossChainOperations.RemoveRange(ops);
                    await _dbContext.SaveChangesAsync();
                }

                return Result.Success();
            }
        }
    }
}
