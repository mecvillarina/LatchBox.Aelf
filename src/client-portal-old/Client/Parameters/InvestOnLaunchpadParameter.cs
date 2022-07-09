using FluentValidation;

namespace Client.Parameters
{
    public class InvestOnLaunchpadParameter
    {
        public double LimitAmount { get; set; }
        public double Amount { get; set; }
    }

    public class InvestOnLaunchpadParameterValidator : AbstractValidator<InvestOnLaunchpadParameter>
    {
        public InvestOnLaunchpadParameterValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(v => v.Amount)
                .GreaterThan(0.0).WithMessage(v => $"'Amount' must be greater than or equal to 0.")
                .LessThanOrEqualTo(v => v.LimitAmount).WithMessage(v => $"'Amount' must be less than or equal to Purchase Limit.");
        }
    }
}
