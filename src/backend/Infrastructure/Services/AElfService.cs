﻿using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Infrastructure.DataContracts;
using RestSharp;

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
            var data = Execute<ChainStatusDataContract>(apiUrl, request);

            return new ChainStatusDto()
            {
                ChainId = data.ChainId,
                LongestChainHeight = data.LongestChainHeight,
                LongestChainHash = data.LongestChainHash,
                GenesisBlockHash = data.GenesisBlockHash,
                GenesisContractAddress = data.GenesisContractAddress,
                LastIrreversibleBlockHash = data.LastIrreversibleBlockHash,
                LastIrreversibleBlockHeight = data.LastIrreversibleBlockHeight,
                BestChainHash = data.BestChainHash,
                BestChainHeight = data.BestChainHeight
            };
        }

    }
}
