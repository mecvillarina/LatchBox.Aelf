using Application.Common.Dtos;
using Application.Common.Models;
using Application.Features.CrossChainOperations.Commands.Create;
using Application.Features.CrossChainOperations.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.Managers.Interfaces
{
    public interface ICrossChainOperationManager : IManager
    {
        Task<IResult> CreateAsync(CreateCrossChainOperationCommand contract);
        Task<IResult<List<CrossChainPendingOperationDto>>> GetPendingOperationAsync(GetPendingOperationsQuery contract);
    }
}