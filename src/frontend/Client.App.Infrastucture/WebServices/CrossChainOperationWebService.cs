using Application.Common.Dtos;
using Application.Common.Models;
using Application.Features.CrossChainOperations.Commands.Create;
using Application.Features.CrossChainOperations.Queries;
using Client.App.Infrastructure.Routes;
using Client.App.Infrastructure.WebServices.Base;
using Client.App.Infrastructure.WebServices.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.App.Infrastructure.WebServices
{
    public class CrossChainOperationWebService : WebServiceBase, ICrossChainOperationWebService
    {
        public CrossChainOperationWebService(AppHttpClient appHttpClient) : base(appHttpClient)
        {
        }

        public Task<IResult> CreateAsync(CreateCrossChainOperationCommand contract) => PostAsync(CrossChainOperationEndpoints.Create, contract);
        public Task<IResult<List<CrossChainPendingOperationDto>>> GetPendingOperationsAsync(GetPendingOperationsQuery contract)
        {
            var endpoint = string.Format(CrossChainOperationEndpoints.GetPending, contract.From, contract.IssueChainId, contract.ContractName);
            return GetAsync<List<CrossChainPendingOperationDto>>(endpoint);
        }
    }
}
