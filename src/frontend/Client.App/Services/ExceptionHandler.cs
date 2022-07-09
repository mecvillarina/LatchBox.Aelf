using Application.Common.Models;
using Client.Infrastructure.Exceptions;
using System;
using System.Threading.Tasks;

namespace Client.App.Services
{
    public class ExceptionHandler : IExceptionHandler
    {
        public ExceptionHandler()
        {
        }

        public async Task HandlerRequestTaskAsync(Func<Task> task)
        {
            try
            {
                await task();
            }
            catch (SessionExpiredException)
            {

            }
        }

        public async Task<IResult> HandlerRequestTaskAsync(Func<Task<IResult>> task)
        {
            try
            {
                var result = await task();

                if (!result.Succeeded)
                {
                    throw new ApiOkFailedException(result.Messages);
                }

                return result;
            }
            catch (SessionExpiredException)
            {
                return await Result.FailAsync();
            }
        }

        public async Task<IResult<TResult>> HandlerRequestTaskAsync<TResult>(Func<Task<IResult<TResult>>> task)
        {
            try
            {
                var result = await task();

                if (!result.Succeeded)
                {
                    throw new ApiOkFailedException(result.Messages);
                }

                return result;
            }
            catch (SessionExpiredException)
            {
                return await Result<TResult>.FailAsync();
            }
        }
    }
}
