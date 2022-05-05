using FluentValidation;

namespace Client.Parameters
{
    public class AddExistingTokenParameter
    {
        public string Symbol { get; set; }

    }

    public class AddExistingTokenParameterValidator : AbstractValidator<AddExistingTokenParameter>
    {
        public AddExistingTokenParameterValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(v => v.Symbol)
                .NotNull()
                .NotEmpty();
        }
    }
}
