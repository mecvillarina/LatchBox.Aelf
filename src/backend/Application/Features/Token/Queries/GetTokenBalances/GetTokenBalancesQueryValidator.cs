using AElf;
using FluentValidation;

namespace Application.Features.Token.Queries.GetTokenBalances
{
    public class GetTokenBalancesQueryValidator : AbstractValidator<GetTokenBalancesQuery>
    {
        public GetTokenBalancesQueryValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(v => v.ChainIdBase58)
                .NotNull().WithMessage("'Chain Id' must not be empty.")
                .NotEmpty().WithMessage("'Chain Id' must not be empty.");

            RuleFor(v => v.Address)
                .NotNull().WithMessage("'Wallet Address' must not be empty.")
                .NotEmpty().WithMessage("'Wallet Address' must not be empty.")
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
                }).WithMessage("Invalid 'Wallet Address' format.");
        }
    }
}
