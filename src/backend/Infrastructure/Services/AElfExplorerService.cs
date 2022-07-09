using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Infrastructure.DataContracts;
using RestSharp;
using RestSharp.Serializers.Json;
using System.Collections.Generic;

namespace Infrastructure.Services
{
    public class AElfExplorerService : IAElfExplorerService
    {
        private readonly RestClient _client;
        public AElfExplorerService()
        {
            _client = new RestClient();
            _client.UseSystemTextJson();
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

        public List<TokenDto> GetTokenList(string explorerUrl)
        {
            var request = new RestRequest("api/viewer/getAllTokens?total=0&pageSize=0&pageNum=1", Method.Get);
            var response = Execute<TokenListDataContract>(explorerUrl, request);

            if (response.Msg != "success") return new List<TokenDto>();

            var total = response.Data.Total;

            request = new RestRequest($"api/viewer/getAllTokens?total={total}&pageSize={total}&pageNum=1", Method.Get);
            response = Execute<TokenListDataContract>(explorerUrl, request);

            if (response.Msg != "success") return new List<TokenDto>();

            var tokens = new List<TokenDto>();
            foreach (var token in response.Data.Tokens)
            {
                tokens.Add(new TokenDto()
                {
                    Id = token.Id,
                    ContractAddress = token.ContractAddress,
                    Symbol = token.Symbol,
                    ChainId = token.ChainId,
                    IssueChainId = token.IssueChainId,
                    TxId = token.TxId,
                    Name = token.Name,
                    TotalSupply = token.TotalSupply,
                    Supply = token.Supply,
                    Decimals = token.Decimals
                });
            }

            return tokens;
        }
    }
}
