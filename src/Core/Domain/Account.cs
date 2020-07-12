using System;
using System.Threading.Tasks;
using Equatable;

namespace Buckpal.Core.Domain
{
    [ImplementsEquatable]
    [ToString]
    public sealed class Account
    {
        public static Account ExistingOf(AccountId id, Money balance) => new Account(id, balance);

        [Equals] private AccountId Id { get; }
        private Money Balance { get; set; }

        private Account(AccountId id, Money balance)
        {
            Id = id;
            Balance = balance;
        }

        public async Task Withdraw(Money money, Func<Task> onFailure)
        {
            if (money > Balance)
            {
                await onFailure();

                throw new AccountInsufficientFundsException(this);
            }

            Balance -= money;
        }

        public void Deposit(Money money)
        {
            Balance += money;
        }

        public void Deconstruct(out AccountId id, out Money balance)
        {
            id = Id;
            balance = Balance;
        }
    }
}