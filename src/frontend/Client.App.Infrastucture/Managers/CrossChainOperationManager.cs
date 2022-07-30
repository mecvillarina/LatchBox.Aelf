using Application.Common.Dtos;
using Application.Common.Models;
using Application.Features.CrossChainOperations.Commands.Confirm;
using Application.Features.CrossChainOperations.Commands.Create;
using Application.Features.CrossChainOperations.Queries;
using Client.App.Infrastructure.Managers.Interfaces;
using Client.App.Infrastructure.WebServices.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.Managers
{
    public class CrossChainOperationManager : ManagerBase, ICrossChainOperationManager
    {
        private readonly ICrossChainOperationWebService _crossChainOperationWebService;
        public CrossChainOperationManager(IManagerToolkit managerToolkit, ICrossChainOperationWebService crossChainOperationWebService) : base(managerToolkit)
        {
            _crossChainOperationWebService = crossChainOperationWebService;
        }

        public async Task<IResult> CreateAsync(CreateCrossChainOperationCommand contract)
        {
            return await _crossChainOperationWebService.CreateAsync(contract);
        }

        public async Task<IResult<List<CrossChainPendingOperationDto>>> GetPendingOperationAsync(GetPendingOperationsQuery contract)
        {
            return await _crossChainOperationWebService.GetPendingOperationsAsync(contract);
        }

        public async Task<IResult> ConfirmAsync(ConfirmCrossChainOperationCommand contract)
        {
            return await _crossChainOperationWebService.ConfirmAsync(contract);
        }

    }
}
