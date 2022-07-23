using AElf;
using FluentValidation;
using System;

namespace Client.App.Parameters
{
    public class AddVestingReceiverParameter
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ReceiverAddress { get; set; }
        public double Amount { get; set; }
    }

    public class AddVestingReceiverParameterValidator : AbstractValidator<AddVestingReceiverParameter>
    {
        public AddVestingReceiverParameterValidator()
        {
            CascadeMode = CascadeMode.Stop;

            RuleFor(v => v.Name)
                .NotNull()
                .NotEmpty();

            RuleFor(v => v.ReceiverAddress)
                .NotNull().WithMessage("'Receiver Address' must not be empty.")
                .NotEmpty().WithMessage("'Receiver Address' must not be empty.")
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
                }).WithMessage("Invalid 'Receiver Address' format.");

            RuleFor(v => v.Amount)
                .GreaterThan(0.0).WithMessage("'Amount' must be greater than 0");

        }
    }
}
