using Application.Common.Constants;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Chain.Commands;
using Application.Features.CrossChainOperations.Commands.Cleanup;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebJob.Base;

namespace WebJob.Functions
{
    public class CrossChainOperationFunction : FunctionBase
    {
        public CrossChainOperationFunction(IMediator mediator, ICallContext context) : base(mediator, context)
        {
        }

        [FunctionName("CrossChainOperation_Cleanup")]
        public async Task Cleanup([TimerTrigger("0 0 0 * * *")] TimerInfo myTimer, ILogger log, ExecutionContext context)
        {
            var commandArg = new CleanupCrossChainOperationCommand() { };
            await ExecuteAsync<CleanupCrossChainOperationCommand, IResult>(context, commandArg);
        }

    }
}
