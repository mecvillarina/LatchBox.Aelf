using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.CrossChainOperations.Commands.Create;
using Application.Features.CrossChainOperations.Queries;
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
    public class CrossChainOperationController : HttpFunctionBase
    {
        public CrossChainOperationController(IConfiguration configuration, IMediator mediator, ICallContext context) : base(configuration, mediator, context)
        {
        }

        [FunctionName("CrossChainOperation_Create")]
        public async Task<IActionResult> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "crosschain/operation/create")] CreateCrossChainOperationCommand commandArgs, HttpRequest req, ExecutionContext context, ILogger logger)
        {
            return await ExecuteAsync<CreateCrossChainOperationCommand, IResult>(context, logger, req, commandArgs);
        }

        [FunctionName("CrossChainOperation_GetPending")]
        public async Task<IActionResult> GetPending([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "crosschain/operation/pending")] GetPendingOperationsQuery queryArgs, HttpRequest req, ExecutionContext context, ILogger logger)
        {
            return await ExecuteAsync<GetPendingOperationsQuery, Result<List<CrossChainPendingOperationDto>>>(context, logger, req, queryArgs);
        }
    }
}
