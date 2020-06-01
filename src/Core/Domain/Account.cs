using System;

namespace Buckpal.Core.Domain
{
    public class Account
    {
        private AccountId Id { get; }
        private Money Balance { get; set; }

        public Account(AccountId id, Money balance)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Balance = balance ?? throw new ArgumentNullException(nameof(balance));
        }

        public void Withdraw(Money money)
        {
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

        public override string ToString() => $"{nameof(Account)} {{ {nameof(Id)}: {Id} }}";

        public static bool operator ==(Account left, Account right) => Equals(left, right);

        public static bool operator !=(Account left, Account right) => !Equals(left, right);
    }
}