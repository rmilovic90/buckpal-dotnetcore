using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Buckpal.Core.Domain
{
    [Trait("Category", "Unit")]
    public sealed class AccountTests
    {
        [Fact]
        public void Requires_id_for_existing_account()
        {
            Action createAccount = () => Account.ExistingOf(null, Money.Of(10.5M));

            createAccount.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Requires_balance_for_existing_account()
        {
            Action createAccount = () => Account.ExistingOf(AccountId.Of(1L), null);

            createAccount.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Withdraws_from_balance()
        {
            var account = Account.ExistingOf(AccountId.Of(1L), Money.Of(10.7M));

            account.Withdraw(Money.Of(1.5M), NoAction);

            var (_, balance) = account;
            balance.Should().Be(Money.Of(9.2M));
        }

        [Fact]
        public async Task Fails_to_withdraw_when_no_sufficient_funds()
        {
            var account = Account.ExistingOf(AccountId.Of(1L), Money.Of(10.7M));

            Func<Task> withdrawMoneyFromAccount = () => account.Withdraw(Money.Of(15M), NoAction);

            (await withdrawMoneyFromAccount.Should().ThrowExactlyAsync<AccountInsufficientFundsException>())
                .Which.Account.Should().Be(account);
        }

        [Fact]
        public async Task Executes_given_withdrawal_failure_action()
        {
            var withdrawalFailureActionExecuted = false;
            Func<Task> withdrawalFailureAction = () =>
            {
                withdrawalFailureActionExecuted = true;

                return Task.CompletedTask;
            };
            var account = Account.ExistingOf(AccountId.Of(1L), Money.Of(10.7M));

            Func<Task> withdrawMoneyFromAccount = () => account.Withdraw(Money.Of(15M), withdrawalFailureAction);

            await withdrawMoneyFromAccount.Should().ThrowExactlyAsync<AccountInsufficientFundsException>();
            withdrawalFailureActionExecuted.Should().BeTrue();
        }

        [Fact]
        public void Deposits_from_balance()
        {
            var account = Account.ExistingOf(AccountId.Of(1L), Money.Of(10.7M));

            account.Deposit(Money.Of(1.5M));

            var (_, balance) = account;
            balance.Should().Be(Money.Of(12.2M));
        }

        private static Task NoAction() => Task.CompletedTask;
    }
}