using Buckpal.Core.Application.Ports.Input;
using Buckpal.Core.Application.Ports.Output;

namespace Buckpal.Core.Application.Services
{
    public static class ApplicationServicesFactory
    {
        public static ISendMoney CreateSendMoneyService(ILoadAccount loadAccount, ILockAccount lockAccount,
            IUpdateAccountState updateAccountState, IUnitOfWork unitOfWork) =>
            new SendMoneyService(loadAccount, lockAccount, updateAccountState, unitOfWork);
    }
}