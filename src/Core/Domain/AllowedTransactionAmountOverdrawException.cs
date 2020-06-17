using System;

namespace Buckpal.Core.Domain
{
    public sealed class AllowedTransactionAmountOverdrawException : Exception
    {
        public AllowedTransactionAmountOverdrawException(Money transactionAmount, Money maximumAllowedTransactionAmount)
            : base($"Transaction amount {transactionAmount} is higher than maximum allowed amount {maximumAllowedTransactionAmount}.")
        {
            TransactionAmount = transactionAmount;
            MaximumAllowedTransactionAmount = maximumAllowedTransactionAmount;
        }

        public Money TransactionAmount { get; }
        public Money MaximumAllowedTransactionAmount { get; }
    }
}