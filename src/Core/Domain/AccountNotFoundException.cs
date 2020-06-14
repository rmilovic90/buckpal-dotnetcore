using System;

namespace Buckpal.Core.Domain
{
    public sealed class AccountNotFoundException : Exception
    {
        public AccountNotFoundException(AccountId id)
            : base($"Account with ID {id} does not exist.") =>
                Id = id;

        public AccountId Id { get; }
    }
}