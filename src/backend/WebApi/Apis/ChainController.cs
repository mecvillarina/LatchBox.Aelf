using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Chain.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WebApi.Base;

namespace WebApi.Apis
{
    public class ChainController : HttpFunctionBase
    {
        public ChainController(IConfiguration configuration, IMediator mediator, ICallContext context) : base(configuration, mediator, context)
        {
        }

        [FunctionName("Chain_UpdateChainInfo")]
        public async Task<IActionResult> UpdateChainInfo([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chain/updateChainInfo")] UpdateChainInfoCommand queryArgs, HttpRequest req, ExecutionContext context, ILogger logger)
        {
            return await ExecuteAsync<UpdateChainInfoCommand, IResult>(context, logger, req, queryArgs);
        }
    }
}
