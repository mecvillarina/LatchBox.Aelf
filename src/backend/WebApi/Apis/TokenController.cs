using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Chain.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Base;
using Application.Features.Token.Queries;
using Application.Common.Dtos;

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
    }
}
