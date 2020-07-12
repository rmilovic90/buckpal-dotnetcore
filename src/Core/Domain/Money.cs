using System;
using System.Globalization;
using Equatable;

namespace Buckpal.Core.Domain
{
    [ImplementsEquatable]
    public sealed class Money
    {
        public static Money Of(decimal amount)
        {
            if (amount < decimal.Zero)
                throw new ArgumentException(
                    $"{nameof(Money)} ${amount} can't be less than {decimal.Zero}.",
                    nameof(amount));

            return new Money(amount);
        }

        [Equals] private readonly decimal _amount;

        private Money(decimal amount) => _amount = amount;

        public override string ToString() => _amount.ToString(CultureInfo.InvariantCulture);

        public static implicit operator decimal(Money money) => money._amount;

        public static Money operator +(Money left, Money right) => new Money(left._amount + right._amount);

        public static Money operator -(Money left, Money right) => new Money(left._amount - right._amount);
    }
}