namespace Buckpal.Core.Application.Ports.Input
{
    public static class Errors
    {
        public static class SendMoney
        {
            public static readonly string AccountIdMustBeGreaterThanZero =
                $"{nameof(SendMoney)}{nameof(AccountIdMustBeGreaterThanZero)}";
            public static readonly string AmountMustBeGreaterThanZero =
                $"{nameof(SendMoney)}{nameof(AmountMustBeGreaterThanZero)}";
        }
    }
}