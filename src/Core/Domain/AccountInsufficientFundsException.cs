using System;

namespace Buckpal.Core.Domain
{
    public sealed class AccountInsufficientFundsException : Exception
    {
        public AccountInsufficientFundsException(Account account)
            : base("Account does not have sufficient funds.") =>
                Account = account;

        public Account Account { get; }
    }
}