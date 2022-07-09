using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using RestSharp;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class AElfService : IAElfService
    {
        private readonly RestClient _client;
        public AElfService()
        {
            _client = new RestClient();
        }

        private void Execute(string apiUrl, RestRequest request)
        {
            _client.Options.BaseUrl = new System.Uri(apiUrl);
            var response = _client.Execute(request);
            ProcessResponse(response);
        }

        private T Execute<T>(string apiUrl, RestRequest request)
        {
            _client.Options.BaseUrl = new System.Uri(apiUrl);
            var response = _client.Execute<T>(request);
            ProcessResponse(response);
            return response.Data;
        }

        private void ProcessResponse(RestResponse response)
        {
            if (!response.IsSuccessful)
            {
                throw new RpcServerErrorException("There's a problem on RPC Service. Please try again.");
            }
        }

        public ChainStatusDto GetChainStatus(string apiUrl)
        {
            var request = new RestRequest("api/blockChain/chainStatus", Method.Get);
            return Execute<ChainStatusDto>(apiUrl, request);
        }

    }
}
