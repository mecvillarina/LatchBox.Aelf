using Application.Common.Extensions;
using Client.App.Parameters;
using Google.Protobuf.WellKnownTypes;
using System;

namespace Client.App.SmartContractDto.Launchpad
{
    public class CrowdSaleCreateInput
    {
        public string Name { get; set; }
        public string TokenSymbol { get; set; }
        public long SoftCapNativeTokenAmount { get; set; }
        public long HardCapNativeTokenAmount { get; set; }
        public long TokenAmountPerNativeToken { get; set; }
        public long NativeTokenPurchaseLimitPerBuyerAddress { get; set; }
        public Timestamp SaleStartDate { get; set; }
        public Timestamp SaleEndDate { get; set; }
        public long LockUntilDurationInMinutes { get; set; }

        public CrowdSaleCreateInput(CreateLaunchpadParameter parameter)
        {
            Name = parameter.Name;
            TokenSymbol = parameter.TokenSymbol;

            SoftCapNativeTokenAmount = parameter.SoftCapNativeTokenAmount.ToChainAmount(parameter.NativeTokenDecimals);
            HardCapNativeTokenAmount = parameter.HardCapNativeTokenAmount.ToChainAmount(parameter.NativeTokenDecimals);
            NativeTokenPurchaseLimitPerBuyerAddress = parameter.NativeTokenPurchaseLimitPerBuyerAddress.ToChainAmount(parameter.NativeTokenDecimals);
            TokenAmountPerNativeToken = parameter.TokenAmountPerNativeToken.ToChainAmount(parameter.TokenDecimals);
            LockUntilDurationInMinutes = parameter.LockUntilDurationInMinutes;
            SaleStartDate = Timestamp.FromDateTime(DateTime.SpecifyKind(parameter.SaleStartDate.Value.Date, DateTimeKind.Utc));
            SaleEndDate = Timestamp.FromDateTime(DateTime.SpecifyKind(parameter.SaleEndDate.Value.Date.AddDays(1).AddMilliseconds(-1), DateTimeKind.Utc));
        }
    }
}
