using AElf;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Token.Commands.AddWalletToken
{
    public class AddWalletTokenCommandValidator : AbstractValidator<AddWalletTokenCommand>
    {
        public AddWalletTokenCommandValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(v => v.ChainIdBase58)
                .NotNull().WithMessage("'Chain Id' must not be empty.")
                .NotEmpty().WithMessage("'Chain Id' must not be empty.");

            RuleFor(v => v.WalletAddress)
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

            RuleFor(v => v.TokenSymbol)
                .NotNull().WithMessage("'Token Symbol' must not be empty.")
                .NotEmpty().WithMessage("'Token Symbol' must not be empty.");
        }
    }
}
