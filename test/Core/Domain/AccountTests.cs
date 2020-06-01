using System;
using FluentAssertions;
using Xunit;

namespace Buckpal.Core.Domain
{
    [Trait("Category", "Unit")]
    public sealed class AccountTests
    {
        [Fact]
        public void Requires_id()
        {
            Action createAccount = () => new Account(null, new Money(10.5M));

            createAccount.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Requires_balance()
        {
            Action createAccount = () => new Account(new AccountId(1L), null);

            createAccount.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Withdraws_from_balance()
        {
            var account = new Account(new AccountId(1L), new Money(10.7M));

            account.Withdraw(new Money(1.5M));

            var (_, balance) = account;
            balance.Should().Be(new Money(9.2M));
        }

        [Fact]
        public void Deposits_from_balance()
        {
            var account = new Account(new AccountId(1L), new Money(10.7M));

            account.Deposit(new Money(1.5M));

            var (_, balance) = account;
            balance.Should().Be(new Money(12.2M));
        }
    }
}