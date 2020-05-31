namespace Buckpal.Core.Domain
{
    public sealed class AccountId
    {
        private readonly long _value;

        public AccountId(long value) => _value = value;

        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || obj is AccountId other && Equals(other);

        private bool Equals(AccountId other) => _value == other._value;

        public override int GetHashCode() => _value.GetHashCode();

        public override string ToString() => _value.ToString();

        public static bool operator ==(AccountId left, AccountId right) => Equals(left, right);

        public static bool operator !=(AccountId left, AccountId right) => !Equals(left, right);

        public static implicit operator long(AccountId accountId) => accountId._value;
    }
}