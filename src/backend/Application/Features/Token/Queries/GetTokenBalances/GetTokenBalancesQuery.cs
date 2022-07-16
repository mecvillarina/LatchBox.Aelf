using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

                var tokenBalances = _explorerService.GetTokenBalanceList(chainInfo.Explorer, request.Address);

                var tokens = _dbContext.TokenInfos.Where(x => x.ChainId == chainInfo.ChainId && x.Issuer == request.Address);

                foreach (var token in tokens)
                {
                    if (!tokenBalances.Any(x => x.Token.Symbol == token.Symbol))
                    {
                        tokenBalances.Add(new TokenBalanceInfoDto()
                        {
                            Token = JsonSerializer.Deserialize<TokenDto>(token.RawExplorerData),
                            Balance = "0",  //to be remove
                            IsIssuer = true
                        });
                    }
                }

                return await Result<List<TokenBalanceInfoDto>>.SuccessAsync(tokenBalances);
            }
        }
    }
}
