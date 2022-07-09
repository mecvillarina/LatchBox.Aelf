using Application.Common.Models;
using System;
using System.Threading.Tasks;

namespace Client.App.Services
{
    public interface IExceptionHandler
    {
        Task HandlerRequestTaskAsync(Func<Task> task);
        Task<IResult> HandlerRequestTaskAsync(Func<Task<IResult>> task);
        Task<IResult<TResult>> HandlerRequestTaskAsync<TResult>(Func<Task<IResult<TResult>>> task);
    }
}