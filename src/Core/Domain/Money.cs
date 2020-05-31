using System;
using System.Globalization;

namespace Buckpal.Core.Domain
{
    public sealed class Money
    {
        public static Money Zero => new Money(decimal.Zero);

        private readonly decimal _amount;

        public Money(decimal amount)
        {
            if (amount < decimal.Zero)
                throw new ArgumentException(
                    $"{nameof(Money)} ${amount} can't be less than {decimal.Zero}.",
                    nameof(amount));

            _amount = amount;
        }

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is Money other && Equals(other);

        private bool Equals(Money other) => _amount == other._amount;

        public override int GetHashCode() => _amount.GetHashCode();

        public override string ToString() => _amount.ToString(CultureInfo.InvariantCulture);

        public static bool operator ==(Money left, Money right) => Equals(left, right);

        public static bool operator !=(Money left, Money right) => !Equals(left, right);

        public static implicit operator decimal(Money money) => money._amount;
    }
}