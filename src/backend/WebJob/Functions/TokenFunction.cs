using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Chain.Commands;
using Application.Features.Token.Commands.SyncTokens;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebJob.Base;

namespace WebJob.Functions
{
    public class TokenFunction : FunctionBase
    {
        public TokenFunction(IMediator mediator, ICallContext context) : base(mediator, context)
        {
        }

        [FunctionName("Token_AELF_SyncToken")]
        public async Task AELFUpdateChainInfo([TimerTrigger("0 * * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var commandArg = new SyncTokenCommand() { ChainIdBase58 = "AELF" };
            await ExecuteAsync<SyncTokenCommand, IResult>(context, commandArg);
        }

        [FunctionName("Token_tDVV_SyncToken")]
        public async Task tDVVUpdateChainInfo([TimerTrigger("20 * * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var commandArg = new SyncTokenCommand() { ChainIdBase58 = "tDVV" };
            await ExecuteAsync<SyncTokenCommand, IResult>(context, commandArg);
        }

        [FunctionName("Token_tDVW_SyncToken")]
        public async Task tdVWUpdateChainInfo([TimerTrigger("40 * * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var commandArg = new SyncTokenCommand() { ChainIdBase58 = "tDVW" };
            await ExecuteAsync<SyncTokenCommand, IResult>(context, commandArg);
        }
    }
}
