using System;

namespace Client.Infrastructure.Models.Inputs
{
    public class CreateLaunchpadInputModel
    {
        public string Name { get; set; }
        public string TokenSymbol { get; set; }
        public long SoftCapNativeTokenAmount { get; set; }
        public long HardCapNativeTokenAmount { get; set; }
        public long TokenAmountPerNativeToken { get; set; }
        public long NativeTokenPurchaseLimitPerBuyerAddress { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }
        public long LockUntilDurationInMinutes { get; set; }
    }
}
