using Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using WebApi.Base;

namespace WebApi.Apis.General
{
    public class GeneralController : HttpFunctionBase
    {
        public GeneralController(IConfiguration configuration, IMediator mediator, ICallContext context) : base(configuration, mediator, context)
        {
        }

        //[FunctionName("General_SampleData")]
        //public async Task<IActionResult> GetSampleData([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "general/coursedata")] GetSampleDataQuery queryArgs, HttpRequest req, ExecutionContext context, ILogger logger)
        //{
        //    return await ExecuteAsync<GetSampleDataQuery, Result<GetSampleDataResponseDto>>(context, logger, req, queryArgs);
        //}

    }
}
