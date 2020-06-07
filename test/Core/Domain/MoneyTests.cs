using System;
using FluentAssertions;
using Xunit;

namespace Buckpal.Core.Domain
{
    [Trait("Category", "Unit")]
    public sealed class MoneyTests
    {
        [Fact]
        public void Requires_positive_or_zero_amount()
        {
            Action createMoney = () => Money.Of(-0.5M);

            createMoney.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Adds_amounts()
        {
            var left = Money.Of(0.6M);
            var right = Money.Of(2.3M);

            var result = left + right;

            result.Should().Be(Money.Of(2.9M));
        }

        [Fact]
        public void Subtracts_amounts()
        {
            var left = Money.Of(3.1M);
            var right = Money.Of(1.9M);

            var result = left - right;

            result.Should().Be(Money.Of(1.2M));
        }
    }
}