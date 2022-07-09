using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Token.Queries
{
    public class GetAllTokensQuery : IRequest<Result<List<TokenDto>>>
    {
        public string ChainIdBase58 { get; set; }

        public class GetAllTokensQueryHandler : IRequestHandler<GetAllTokensQuery, Result<List<TokenDto>>>
        {
            private readonly IAElfExplorerService _explorerService;
            private readonly IApplicationDbContext _dbContext;

            public GetAllTokensQueryHandler(IAElfExplorerService explorerService, IApplicationDbContext dbContext)
            {
                _explorerService = explorerService;
                _dbContext = dbContext;
            }

            public async Task<Result<List<TokenDto>>> Handle(GetAllTokensQuery request, CancellationToken cancellationToken)
            {
                var chainInfo = _dbContext.ChainInfos.FirstOrDefault(x => x.ChainIdBase58 == request.ChainIdBase58);

                if (chainInfo == null || string.IsNullOrEmpty(chainInfo.Explorer)) return await Result<List<TokenDto>>.FailAsync("Chain not supported.");

                var data = _explorerService.GetTokenList(chainInfo.Explorer);

                return await Result<List<TokenDto>>.SuccessAsync(data);
            }
        }
    }
}
