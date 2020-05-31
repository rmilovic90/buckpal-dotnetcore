using System;

namespace Buckpal.Core.Domain
{
    public class Account
    {
        public Account(AccountId id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public AccountId Id { get; }
        public Money Balance { get; } = Money.Zero;

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is Account other && Equals(other);

        private bool Equals(Account other) => Id.Equals(other.Id);

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => $"{nameof(Account)} {{ {nameof(Id)}: {Id} }}";

        public static bool operator ==(Account left, Account right) => Equals(left, right);

        public static bool operator !=(Account left, Account right) => !Equals(left, right);
    }
}