using FluentValidation;

namespace Client.App.Parameters
{
    public class CreateTokenParameter
    {
        public CreateTokenParameter(bool isBurnable, int issueChainId)
        {
            IsBurnable = isBurnable;
            IssueChainId = issueChainId;
        }
        public string Symbol { get; set; } = "FIRE";
        public string TokenName { get; set; } = "FIRE";
        public long TotalSupply { get; set; } = 21000000;
        public int Decimals { get; set; } = 8;
        public bool IsBurnable { get; set; }
        public int IssueChainId { get; set; }
    }

    public class CreateTokenParameterValidator : AbstractValidator<CreateTokenParameter>
    {
        public CreateTokenParameterValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(v => v.Symbol)
                .NotNull()
                .NotEmpty()
                .MinimumLength(3).WithMessage("'Symbol' must be 3-8 characters.")
                .MaximumLength(8).WithMessage("'Symbol' must be 3-8 characters.");


            RuleFor(v => v.TokenName)
                .NotNull()
                .NotEmpty()
                .MinimumLength(3).WithMessage("'Name' must be at least 3 characters long.");

            RuleFor(v => v.Decimals)
                .GreaterThanOrEqualTo(0).WithMessage("'Decimals' must be greater than 0.");

            RuleFor(v => v.TotalSupply)
                .GreaterThan(0).WithMessage("'Total Supply' must be greater than 0.");

            RuleFor(v => v.IssueChainId)
                .NotNull().WithMessage("'Issued Chain' must not be empty.")
                .NotEmpty().WithMessage("'Issued Chain' must not be empty.");
            //RuleFor(v => v.InitialSupply)
            //    .GreaterThanOrEqualTo(0).WithMessage("'Initial Supply' must be greater than or equal to 0.")
            //    .LessThanOrEqualTo(v => v.TotalSupply).WithMessage("'Initial Supply' must be less than or equal to the 'Total Supply'.");
        }
    }
}
