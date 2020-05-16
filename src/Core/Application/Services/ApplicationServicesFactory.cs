using Buckpal.Core.Application.Ports.Input;

namespace Buckpal.Core.Application.Services
{
    public static class ApplicationServicesFactory
    {
        public static ISendMoney CreateSendMoneyService() => new SendMoneyService();
    }
}