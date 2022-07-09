using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Token.Commands.RemoveWalletToken
{
    public class RemoveWalletTokenCommand : IRequest<IResult>
    {
        public string ChainIdBase58 { get; set; }
        public string WalletAddress { get; set; }
        public string TokenSymbol { get; set; }

        public class RemoveWalletTokenCommandHandler : IRequestHandler<RemoveWalletTokenCommand, IResult>
        {
            private readonly IApplicationDbContext _dbContext;

            public RemoveWalletTokenCommandHandler(IApplicationDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<IResult> Handle(RemoveWalletTokenCommand request, CancellationToken cancellationToken)
            {
                var walletToken = _dbContext.WalletTokens.FirstOrDefault(x => x.WalletAddress == request.WalletAddress && x.ChainIdBase58 == request.ChainIdBase58 && x.TokenSymbol == request.TokenSymbol);

                if (walletToken != null)
                {
                    _dbContext.WalletTokens.Remove(walletToken);
                    _dbContext.SaveChanges();
                }

                return await Result.SuccessAsync();
            }
        }
    }
}
