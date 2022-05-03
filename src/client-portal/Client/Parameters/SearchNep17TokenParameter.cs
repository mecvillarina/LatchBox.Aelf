using AElf;
using Client.Infrastructure.Managers.Interfaces;
using FluentValidation;

namespace Client.Parameters
{
    public class SearchNep17TokenParameter
    {
        public string TokenScriptHash { get; set; }
    }

    public class SearchNep17TokenParameterValidator : AbstractValidator<SearchNep17TokenParameter>
    {
        public SearchNep17TokenParameterValidator(IManagerToolkit managerToolkit)
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(v => v.TokenScriptHash)
                .NotNull().WithMessage("'Token ScriptHash' must not be empty.")
                .NotEmpty().WithMessage("'Token ScriptHash' must not be empty.")
                .Must(x =>
                {
                    try
                    {
                        return AddressHelper.VerifyFormattedAddress(x);
                    }
                    catch
                    {
                        return false;
                    }
                }).WithMessage("Invalid 'Token ScriptHash' format.");
        }
    }
}
