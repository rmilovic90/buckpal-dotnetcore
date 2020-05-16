using FluentValidation;

namespace Buckpal.Core.Application.Ports.Input
{
    public sealed class SendMoneyCommand : SelfValidate<SendMoneyCommand>
    {
        public SendMoneyCommand(
            long sourceAccountId,
            long targetAccountId,
            decimal amount) : base(new Validator())
        {
            SourceAccountId = sourceAccountId;
            TargetAccountId = targetAccountId;
            Amount = amount;

            ValidateSelf();
        }

        public long SourceAccountId { get; }
        public long TargetAccountId { get; }
        public decimal Amount { get; }

        private class Validator : AbstractValidator<SendMoneyCommand>
        {
            public Validator()
            {
                RuleFor(command => command.SourceAccountId)
                    .GreaterThan(0)
                    .WithErrorCode(Errors.SendMoney.AccountIdMustBeGreaterThanZero);
                RuleFor(command => command.TargetAccountId)
                    .GreaterThan(0)
                    .WithErrorCode(Errors.SendMoney.AccountIdMustBeGreaterThanZero);
                RuleFor(command => command.Amount)
                    .GreaterThan(0)
                    .WithErrorCode(Errors.SendMoney.AmountMustBeGreaterThanZero);
            }
        }
    }
}