using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Chain.Commands
{
    public class UpdateChainInfoCommand : IRequest<IResult>
    {
        public string ChainIdBase58 { get; set; }

        public class UpdateChainInfoCommandHandler : IRequestHandler<UpdateChainInfoCommand, IResult>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IAElfService _aElfService;
            private readonly IDateTime _dateTime;

            public UpdateChainInfoCommandHandler(IApplicationDbContext dbContext, IAElfService aElfService, IDateTime dateTime)
            {
                _dbContext = dbContext;
                _aElfService = aElfService;
                _dateTime = dateTime;
            }

            public async Task<IResult> Handle(UpdateChainInfoCommand request, CancellationToken cancellationToken)
            {
                var chainInfo = _dbContext.ChainInfos.FirstOrDefault(x => x.ChainIdBase58 == request.ChainIdBase58);

                if (chainInfo == null || string.IsNullOrEmpty(chainInfo.RpcApi)) return await Result.FailAsync("Chain not supported.");

                var chainStatus = _aElfService.GetChainStatus(chainInfo.RpcApi);

                chainInfo.LongestChainHeight = chainStatus.LongestChainHeight;
                chainInfo.LongestChainHash = chainStatus.LongestChainHash;
                chainInfo.GenesisBlockHash = chainStatus.GenesisBlockHash;
                chainInfo.GenesisContractAddress = chainStatus.GenesisContractAddress;
                chainInfo.LastIrreversibleBlockHash = chainStatus.LastIrreversibleBlockHash;
                chainInfo.LastIrreversibleBlockHeight = chainStatus.LastIrreversibleBlockHeight;
                chainInfo.BestChainHash = chainStatus.BestChainHash;
                chainInfo.BestChainHeight = chainStatus.BestChainHeight;
                chainInfo.LastUpdate = _dateTime.UtcNow;

                _dbContext.ChainInfos.Update(chainInfo);
                await _dbContext.SaveChangesAsync();

                return await Result.SuccessAsync();
            }
        }
    }
}
