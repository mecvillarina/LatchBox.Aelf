using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Localization;
using Application.Common.Models;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnauthorizedAccessException = Application.Common.Exceptions.UnauthorizedAccessException;

namespace WebJob.Base
{
    public abstract class FunctionBase
    {
        protected readonly ICallContext Context;
        protected readonly IMediator Mediator;

        protected FunctionBase(IMediator mediator, ICallContext context)
        {
            Mediator = mediator;
            Context = context;
        }

        protected async Task<IActionResult> ExecuteAsync<TRequest, TResponse>(ExecutionContext executionContext, TRequest request,
                Func<TResponse, Task<IActionResult>> resultMethod = null)
                where TRequest : IRequest<TResponse>
        {
            try
            {
                Context.CorrelationId = executionContext.InvocationId;
                Context.FunctionName = executionContext.FunctionName;

                var response = await Mediator.Send(request);

                if (resultMethod != null)
                    return await resultMethod(response);

                return new OkObjectResult(response);
            }
            catch (ValidationException validationException)
            {
                var errors = new List<string>();
                foreach (var validationError in validationException.Errors) errors.AddRange(validationError.Value);

                var result = await Result.FailAsync(errors);
                return new OkObjectResult(result);
            }
            catch (NotFoundException)
            {
                var result = await Result.FailAsync("The specified resource was not found.");
                return new OkObjectResult(result);
            }
            catch (ForbiddenAccessException)
            {
                var result = await Result.FailAsync("You don't have access to this feature.");
                return new OkObjectResult(result);
            }
            catch (UnauthorizedAccessException)
            {
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                var result = await Result.FailAsync(LocalizationResource.Error_GenericServerMessage);
                return new OkObjectResult(result);

            }
        }
    }
}