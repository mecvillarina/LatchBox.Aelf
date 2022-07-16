using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Chain.Commands;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebJob.Base;

namespace WebJob.Functions
{
    public class ChainFunction : FunctionBase
    {
        public ChainFunction(IMediator mediator, ICallContext context) : base(mediator, context)
        {
        }

        [FunctionName("Chain_AELF_UpdateChainInfo")]
        public async Task AELFUpdateChainInfo([TimerTrigger("0 * * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var commandArg = new UpdateChainInfoCommand() { ChainIdBase58 = "AELF" };
            await ExecuteAsync<UpdateChainInfoCommand, IResult>(context, commandArg);
        }

        [FunctionName("Chain_tDVV_UpdateChainInfo")]
        public async Task tDVVUpdateChainInfo([TimerTrigger("20 * * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var commandArg = new UpdateChainInfoCommand() { ChainIdBase58 = "tDVV" };
            await ExecuteAsync<UpdateChainInfoCommand, IResult>(context, commandArg);
        }

        [FunctionName("Chain_tDVW_UpdateChainInfo")]
        public async Task tdVWUpdateChainInfo([TimerTrigger("40 * * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var commandArg = new UpdateChainInfoCommand() { ChainIdBase58 = "tDVW" };
            await ExecuteAsync<UpdateChainInfoCommand, IResult>(context, commandArg);
        }
    }
}
