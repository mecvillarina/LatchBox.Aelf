using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using Domain.Entities;
using MediatR;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Token.Commands.SyncTokens
{
    public class SyncTokenCommand : IRequest<IResult>
    {
        public string ChainIdBase58 { get; set; }

        public class SyncTokenCommandHandler : IRequestHandler<SyncTokenCommand, IResult>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IAElfService _aElfService;
            private readonly IDateTime _dateTime;
            private readonly IAElfExplorerService _explorerService;

            public SyncTokenCommandHandler(IApplicationDbContext dbContext, IAElfService aElfService, IDateTime dateTime, IAElfExplorerService explorerService)
            {
                _dbContext = dbContext;
                _aElfService = aElfService;
                _dateTime = dateTime;
                _explorerService = explorerService;
            }

            public async Task<IResult> Handle(SyncTokenCommand request, CancellationToken cancellationToken)
            {
                var chainInfo = _dbContext.ChainInfos.FirstOrDefault(x => x.ChainIdBase58 == request.ChainIdBase58);

                if (chainInfo == null || string.IsNullOrEmpty(chainInfo.Explorer)) return await Result.FailAsync("Chain not supported.");

                var tokens = _explorerService.GetTokenList(chainInfo.Explorer);

                foreach (var token in tokens)
                {
                    if (!_dbContext.TokenInfos.Any(x => x.ChainId == chainInfo.ChainId && x.Symbol == token.Symbol))
                    {
                        TransactionResultDto tx = default!;
                        try
                        {
                            tx = _explorerService.GetTx(chainInfo.Explorer, token.TxId);
                        }
                        catch { }

                        var tokenInfo = new TokenInfo()
                        {
                            ChainId = chainInfo.ChainId,
                            Symbol = token.Symbol,
                            RawExplorerData = JsonSerializer.Serialize(token),
                        };

                        if (tx != null && tx.Transaction != null && tx.ErrorMessage == null)
                        {
                            tokenInfo.Issuer = tx.Transaction.From;
                            tokenInfo.RawTxData = JsonSerializer.Serialize(tx);
                        }

                        _dbContext.TokenInfos.Add(tokenInfo);
                    }
                }

                _dbContext.SaveChanges();

                return await Result.SuccessAsync();
            }
        }
    }
}
