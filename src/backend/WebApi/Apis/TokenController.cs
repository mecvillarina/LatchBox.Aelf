using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Token.Commands.SyncTokens;
using Application.Features.Token.Queries;
using Application.Features.Token.Queries.GetTokenBalances;
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
    public class TokenController : HttpFunctionBase
    {
        public TokenController(IConfiguration configuration, IMediator mediator, ICallContext context) : base(configuration, mediator, context)
        {
        }

        [FunctionName("Token_GetAllTokens")]
        public Task<IActionResult> GetAllTokens([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "token/{ChainIdBase58}/getAllTokens")] GetAllTokensQuery queryArgs, HttpRequest req, ExecutionContext context, ILogger logger)
        {
            return ExecuteAsync<GetAllTokensQuery, Result<List<TokenDto>>>(context, logger, req, queryArgs);
        }

        [FunctionName("Token_GetTokenBalances")]
        public Task<IActionResult> GetTokenBalances([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "token/{ChainIdBase58}/getTokenBalances")] GetTokenBalancesQuery queryArgs, HttpRequest req, ExecutionContext context, ILogger logger)
        {
            return ExecuteAsync<GetTokenBalancesQuery, Result<List<TokenBalanceInfoDto>>>(context, logger, req, queryArgs);
        }

        [FunctionName("Token_SyncToken")]
        public Task<IActionResult> SyncToken([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "token/{ChainIdBase58}/syncToken")] SyncTokenCommand queryArgs, HttpRequest req, ExecutionContext context, ILogger logger)
        {
            return ExecuteAsync<SyncTokenCommand, IResult>(context, logger, req, queryArgs);
        }
    }
}
