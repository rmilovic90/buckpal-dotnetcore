using System;
using System.Threading.Tasks;

namespace Buckpal.Core.Domain
{
    public sealed class Account
    {
        public static Account ExistingOf(AccountId id, Money balance)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            if (balance == null) throw new ArgumentNullException(nameof(balance));

            return new Account(id, balance);
        }

        private AccountId Id { get; }
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

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is Account other && Equals(other);

        private bool Equals(Account other) => Id.Equals(other.Id);

        public override int GetHashCode() => Id.GetHashCode();

        public void Deconstruct(out AccountId id, out Money balance)
        {
            id = Id;
            balance = Balance;
        }

        public override string ToString() => $"{nameof(Account)} {{ {nameof(Id)}: {Id}, {nameof(Balance)}: {Balance} }}";

        public static bool operator ==(Account left, Account right) => Equals(left, right);

        public static bool operator !=(Account left, Account right) => !Equals(left, right);
    }
}