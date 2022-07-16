using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Token.Queries.GetTokenBalances
{
    public class GetTokenBalancesQuery : IRequest<Result<List<TokenBalanceInfoDto>>>
    {
        public string ChainIdBase58 { get; set; }
        public string Address { get; set; }

        public class GetTokenBalancesQueryHandler : IRequestHandler<GetTokenBalancesQuery, Result<List<TokenBalanceInfoDto>>>
        {
            private readonly IAElfExplorerService _explorerService;
            private readonly IApplicationDbContext _dbContext;

            public GetTokenBalancesQueryHandler(IAElfExplorerService explorerService, IApplicationDbContext dbContext)
            {
                _explorerService = explorerService;
                _dbContext = dbContext;
            }

            public async Task<Result<List<TokenBalanceInfoDto>>> Handle(GetTokenBalancesQuery request, CancellationToken cancellationToken)
            {
                var chainInfo = _dbContext.ChainInfos.FirstOrDefault(x => x.ChainIdBase58 == request.ChainIdBase58);

                if (chainInfo == null || string.IsNullOrEmpty(chainInfo.Explorer)) return await Result<List<TokenBalanceInfoDto>>.FailAsync("Chain not supported.");

                var data = _explorerService.GetTokenBalanceList(chainInfo.Explorer, request.Address);

                return await Result<List<TokenBalanceInfoDto>>.SuccessAsync(data);
            }
        }
    }
}
