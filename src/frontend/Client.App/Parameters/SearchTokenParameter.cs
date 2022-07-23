using FluentValidation;

namespace Client.App.Parameters
{
    public class SearchTokenParameter
    {
        public string Symbol { get; set; }

    }

    public class SearchTokenParameterValidator : AbstractValidator<SearchTokenParameter>
    {
        public SearchTokenParameterValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(v => v.Symbol)
                .NotNull()
                .NotEmpty();
        }
    }
}
