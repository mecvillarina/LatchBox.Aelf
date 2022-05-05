﻿using AElf;
using FluentValidation;

namespace Client.Parameters
{
    public class IssueTokenParameter
    {
        public string Symbol { get; set; }
        public string TokenName { get; set; }
        public int Decimals { get; set; }
        public long Amount { get; set; }
        public string To { get; set; }
        public string Memo { get; set; }
    }

    public class IssueTokenParameterValidator : AbstractValidator<IssueTokenParameter>
    {
        public IssueTokenParameterValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(v => v.To)
                .NotNull().WithMessage("'To' must not be empty.")
                .NotEmpty().WithMessage("'To' must not be empty.")
                .Must(x => AddressHelper.VerifyFormattedAddress(x)).WithMessage("Invalid 'To' Address format.");

            RuleFor(v => v.Amount)
               .GreaterThan(0).WithMessage("'Amount' must be greater than 0.");
        }
    }

}
