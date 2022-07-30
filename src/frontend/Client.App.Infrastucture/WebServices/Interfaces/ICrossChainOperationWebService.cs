using Application.Common.Dtos;
using Application.Common.Models;
using Application.Features.CrossChainOperations.Commands.Confirm;
using Application.Features.CrossChainOperations.Commands.Create;
using Application.Features.CrossChainOperations.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.WebServices.Interfaces
{
    public interface ICrossChainOperationWebService : IWebService
    {
        Task<IResult> CreateAsync(CreateCrossChainOperationCommand contract);
        Task<IResult> ConfirmAsync(ConfirmCrossChainOperationCommand contract);
        Task<IResult<List<CrossChainPendingOperationDto>>> GetPendingOperationsAsync(GetPendingOperationsQuery contract);
    }
}