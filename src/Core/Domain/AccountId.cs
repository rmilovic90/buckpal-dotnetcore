using Equatable;

namespace Buckpal.Core.Domain
{
    [ImplementsEquatable]
    public sealed class AccountId
    {
        public static AccountId Of(long value) => new AccountId(value);

        [Equals] private readonly long _value;

        private AccountId(long value) => _value = value;

        public override string ToString() => _value.ToString();

        public static implicit operator long(AccountId accountId) => accountId._value;
    }
}