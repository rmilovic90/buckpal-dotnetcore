namespace Buckpal.Core.Application.Ports.Input
{
    public interface ISendMoney
    {
        bool SendMoney(SendMoneyCommand command);
    }
}