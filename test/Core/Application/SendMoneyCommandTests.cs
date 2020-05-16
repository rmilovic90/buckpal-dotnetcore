using System;
using System.Collections.Generic;
using Buckpal.Core.Application.Ports.Input;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace Buckpal.Core.Application
{
    [Trait("Category", "Unit")]
    public sealed class SendMoneyCommandTests
    {
        [Theory]
        [InlineData(-1L)]
        [InlineData(0L)]
        public void Requires_source_account_id_greater_than_zero(long sourceAccountId)
        {
            Action createSendMoneyCommand = () => new SendMoneyCommand(sourceAccountId, 1L, 0.10M);

            createSendMoneyCommand.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle(
                    failure => failure.PropertyName == nameof(SendMoneyCommand.SourceAccountId)
                        && failure.ErrorCode == Errors.SendMoney.AccountIdMustBeGreaterThanZero);
        }

        [Theory]
        [InlineData(-1L)]
        [InlineData(0L)]
        public void Requires_target_account_id_greater_than_zero(long targetAccountId)
        {
            Action createSendMoneyCommand = () => new SendMoneyCommand(1L, targetAccountId, 0.10M);

            createSendMoneyCommand.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle(
                    failure => failure.PropertyName == nameof(SendMoneyCommand.TargetAccountId)
                        && failure.ErrorCode == Errors.SendMoney.AccountIdMustBeGreaterThanZero);
        }

        [Theory]
        [MemberData(nameof(NegativeMoneyAmount))]
        [MemberData(nameof(ZeroMoneyAmount))]
        public void Requires_money_amount_greater_than_zero(decimal moneyAmount)
        {
            Action createSendMoneyCommand = () => new SendMoneyCommand(1L, 2L, moneyAmount);

            createSendMoneyCommand.Should().Throw<ValidationException>()
                .Which.Errors.Should().ContainSingle(
                    failure => failure.PropertyName == nameof(SendMoneyCommand.Amount)
                        && failure.ErrorCode == Errors.SendMoney.AmountMustBeGreaterThanZero);
        }

        [Fact]
        public void Allows_valid_values()
        {
            Action createSendMoneyCommand = () => new SendMoneyCommand(1L, 2L, 0.10M);

            createSendMoneyCommand.Should().NotThrow();
        }

        public static IEnumerable<object[]> NegativeMoneyAmount
        {
            get { yield return new object[] { -0.10M }; }
        }

        public static IEnumerable<object[]> ZeroMoneyAmount
        {
            get { yield return new object[] { decimal.Zero }; }
        }
    }
}