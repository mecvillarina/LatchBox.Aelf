using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Chain.Commands;
using Application.Features.Chain.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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
        public async Task<IActionResult> UpdateChainInfo([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chain/updateChainInfo")] UpdateChainInfoCommand commandArgs, HttpRequest req, ExecutionContext context, ILogger logger)
        {
            return await ExecuteAsync<UpdateChainInfoCommand, IResult>(context, logger, req, commandArgs);
        }

        [FunctionName("Chain_GetAllSupportedChains")]
        public async Task<IActionResult> GetAllSupportedChains([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "chain/getAllSupportedChains")] GetAllSupportedChainsQuery queryArgs, HttpRequest req, ExecutionContext context, ILogger logger)
        {
            return await ExecuteAsync<GetAllSupportedChainsQuery, Result<List<ChainInfoDto>>>(context, logger, req, queryArgs);
        }

    }
}
