using FluentValidation;

namespace Client.Parameters
{
    public class InvestOnCrowdSaleParameter
    {
        public double LimitAmount { get; set; }
        public double Amount { get; set; }
    }

    public class BuyOnCrowdSaleParameterValidator : AbstractValidator<InvestOnCrowdSaleParameter>
    {
        public BuyOnCrowdSaleParameterValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(v => v.Amount)
                .GreaterThan(0.0).WithMessage(v => $"'Amount' must be greater than or equal to 0.")
                .LessThanOrEqualTo(v => v.LimitAmount).WithMessage(v => $"'Amount' must be less than or equal to Purchase Limit.");
        }
    }
}
