using FluentValidation;

namespace Client.Parameters
{
    public class CreateTokenParameter
    {
        public string Symbol { get; set; }
        public string TokenName { get; set; }
        public long TotalSupply { get; set; }
        //public long InitialSupply { get; set; }
        public int Decimals { get; set; } = 8;
        public bool IsBurnable { get; set; } = true;
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

            //RuleFor(v => v.InitialSupply)
            //    .GreaterThanOrEqualTo(0).WithMessage("'Initial Supply' must be greater than or equal to 0.")
            //    .LessThanOrEqualTo(v => v.TotalSupply).WithMessage("'Initial Supply' must be less than or equal to the 'Total Supply'.");
        }
    }
}
