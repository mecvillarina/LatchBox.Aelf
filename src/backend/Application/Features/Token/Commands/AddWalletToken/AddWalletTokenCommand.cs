using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Token.Commands.AddWalletToken
{
    public class AddWalletTokenCommand : IRequest<IResult>
    {
        public string ChainIdBase58 { get; set; }
        public string WalletAddress { get; set; }
        public string TokenSymbol { get; set; }

        public class AddWalletTokenCommandHandler : IRequestHandler<AddWalletTokenCommand, IResult>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IAElfExplorerService _explorerService;

            public AddWalletTokenCommandHandler(IApplicationDbContext dbContext, IAElfExplorerService explorerService)
            {
                _dbContext = dbContext;
                _explorerService = explorerService;
            }

            public async Task<IResult> Handle(AddWalletTokenCommand request, CancellationToken cancellationToken)
            {
                var chainInfo = _dbContext.ChainInfos.FirstOrDefault(x => x.ChainIdBase58 == request.ChainIdBase58 && x.IsEnabled);

                if (chainInfo == null || string.IsNullOrEmpty(chainInfo.Explorer)) return await Result.FailAsync("Chain not supported.");

                var tokens = _explorerService.GetTokenList(chainInfo.Explorer);
                var token = tokens.FirstOrDefault(x => x.Symbol == request.TokenSymbol);

                if (token == null) return await Result.FailAsync("Token not exists.");

                var walletToken = _dbContext.WalletTokens.FirstOrDefault(x => x.WalletAddress == request.WalletAddress && x.ChainIdBase58 == request.ChainIdBase58 && x.TokenSymbol == request.TokenSymbol);

                if (walletToken == null)
                {
                    walletToken = new WalletToken()
                    {
                        ChainIdBase58 = chainInfo.ChainIdBase58,
                        WalletAddress = request.WalletAddress,
                        TokenName = token.Name,
                        TokenSymbol = token.Symbol,
                        TokenDecimals = Convert.ToInt32(token.Decimals),
                        TokenSupply = token.Supply,
                        TokenTotalSupply = token.TotalSupply
                    };

                    _dbContext.WalletTokens.Add(walletToken);
                    _dbContext.SaveChanges();
                }

                return await Result.SuccessAsync();
            }
        }
    }
}
