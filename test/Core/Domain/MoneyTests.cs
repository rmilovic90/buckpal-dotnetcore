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
            Action createMoney = () => new Money(-0.5M);

            createMoney.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Adds_amounts()
        {
            var left = new Money(0.6M);
            var right = new Money(2.3M);

            var result = left + right;

            result.Should().Be(new Money(2.9M));
        }

        [Fact]
        public void Subtracts_amounts()
        {
            var left = new Money(3.1M);
            var right = new Money(1.9M);

            var result = left - right;

            result.Should().Be(new Money(1.2M));
        }
    }
}