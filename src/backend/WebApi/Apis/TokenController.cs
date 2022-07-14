using Application.Common.Dtos;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Token.Commands.AddWalletToken;
using Application.Features.Token.Commands.RemoveWalletToken;
using Application.Features.Token.Queries;
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
        public Task<IActionResult> GetAllTokens([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "token/getAllTokens/{ChainIdBase58}")] GetAllTokensQuery queryArgs, HttpRequest req, ExecutionContext context, ILogger logger)
        {
            return ExecuteAsync<GetAllTokensQuery, Result<List<TokenDto>>>(context, logger, req, queryArgs);
        }

        [FunctionName("Token_GetTokenBalances")]
        public Task<IActionResult> GetTokenBalances([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "token/getTokenBalances/{ChainIdBase58}")] GetTokenBalancesQuery queryArgs, HttpRequest req, ExecutionContext context, ILogger logger)
        {
            return ExecuteAsync<GetTokenBalancesQuery, Result<List<TokenBalanceInfoDto>>>(context, logger, req, queryArgs);
        }

        [FunctionName("Token_AddWalletToken")]
        public Task<IActionResult> AddWalletToken([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "token/wallet/add")] AddWalletTokenCommand commandArgs, HttpRequest req, ExecutionContext context, ILogger logger)
        {
            return ExecuteAsync<AddWalletTokenCommand, IResult>(context, logger, req, commandArgs);
        }

        [FunctionName("Token_RemoveWalletToken")]
        public Task<IActionResult> RemoveWalletToken([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "token/wallet/remove")] RemoveWalletTokenCommand commandArgs, HttpRequest req, ExecutionContext context, ILogger logger)
        {
            return ExecuteAsync<RemoveWalletTokenCommand, IResult>(context, logger, req, commandArgs);
        }
    }
}
