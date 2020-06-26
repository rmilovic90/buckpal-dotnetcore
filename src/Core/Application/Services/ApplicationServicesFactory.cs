using Buckpal.Core.Application.Ports.Input;
using Buckpal.Core.Application.Ports.Output;

namespace Buckpal.Core.Application.Services
{
    public static class ApplicationServicesFactory
    {
        public static ISendMoney CreateSendMoneyService(IProvideSendMoneyConfiguration provideSendMoneyConfiguration,
            ILoadAccount loadAccount, ILockAccount lockAccount, IUpdateAccountState updateAccountState,
            IUnitOfWork unitOfWork) =>
                new SendMoneyService(provideSendMoneyConfiguration, loadAccount, lockAccount, updateAccountState, unitOfWork);
    }
}