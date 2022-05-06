﻿using AElf.Client.Dto;
using Client.Infrastructure.Models;
using Client.Infrastructure.Models.Inputs;
using System.Threading.Tasks;

namespace Client.Infrastructure.Managers.Interfaces
{
    public interface IMultiCrowdSaleManager : IManager
    {
        string ContactAddress { get; }
        Task<TransactionResultDto> InitializeAsync(WalletInformation wallet, string password);
        Task<TransactionResultDto> CreateAsync(WalletInformation wallet, string password, CreateCrowdSaleInputModel model);
    }
}