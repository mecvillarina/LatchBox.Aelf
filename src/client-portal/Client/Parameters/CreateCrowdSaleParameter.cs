using FluentValidation;

namespace Client.Parameters
{
    public class CreateCrowdSaleParameter
    {
        public string NativeTokenName { get; set; }
        public string NativeTokenSymbol { get; set; }
        public int NativeTokenDecimals { get; set; }
        public int TokenDecimals { get; set; }
        public string TokenName { get; set; }
        public string TokenSymbol { get; set; }

        public string Name { get; set; }
        public double SoftCapNativeTokenAmount { get; set; } = 1;
        public double HardCapNativeTokenAmount { get; set; } = 10;
        public double TokenAmountPerNativeToken { get; set; } = 50;
        public double NativeTokenPurchaseLimitPerBuyerAddress { get; set; } = 1;
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }
        public long LockUntilDurationInMinutes { get; set; } = 60;
    }

    public class CreateCrowdSaleParameterValidator : AbstractValidator<CreateCrowdSaleParameter>
    {
        public CreateCrowdSaleParameterValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(v => v.Name)
                .NotNull()
                .NotEmpty();

            RuleFor(v => v.SoftCapNativeTokenAmount)
                .GreaterThan(0.0).WithMessage(v => $"'Soft Cap ({v.NativeTokenSymbol})' must be greater than or equal to 0.");

            RuleFor(v => v.HardCapNativeTokenAmount)
              .GreaterThan(0).WithMessage(v => $"'Hard Cap ({v.NativeTokenSymbol})' must be greater than or equal to 0.")
              .GreaterThanOrEqualTo(v => v.SoftCapNativeTokenAmount).WithMessage(v => $"'Hard Cap ({v.NativeTokenSymbol})' must be greater than or equal to the 'Soft Cap ({v.NativeTokenSymbol})'.");

            RuleFor(v => v.TokenAmountPerNativeToken)
                .GreaterThan(0.0).WithMessage(v => $"'Token Amount equivalent per {v.NativeTokenSymbol}' must be greater than to 0.");

            RuleFor(v => v.NativeTokenPurchaseLimitPerBuyerAddress)
                .GreaterThan(0.0).WithMessage(v => $"'Token Purchase Limit ({v.NativeTokenSymbol}) per buyer address' must be greater than to 0.")
                .LessThanOrEqualTo(v => v.HardCapNativeTokenAmount).WithMessage(v => $"'Token Purchase Limit ({v.NativeTokenSymbol}) per buyer address' must be less than or equal to the 'Hard Cap ({v.NativeTokenSymbol})'.");

            RuleFor(v => v.LockUntilDurationInMinutes)
                .GreaterThanOrEqualTo(60).WithMessage("'Lock duration after sale must be at least 60 minutes or 1 hour.");

        }
    }
}
