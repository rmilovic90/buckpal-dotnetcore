using System;
using System.Threading.Tasks;
using Buckpal.Core.Application.Ports.Input;
using Buckpal.Core.Application.Ports.Output;
using Buckpal.Core.Domain;

namespace Buckpal.Core.Application.Services
{
    internal sealed class SendMoneyService : ISendMoney
    {
        private readonly ILoadAccount _loadAccount;
        private readonly ILockAccount _lockAccount;
        private readonly IUpdateAccountState _updateAccountState;
        private readonly IUnitOfWork _unitOfWork;

        public SendMoneyService(ILoadAccount loadAccount, ILockAccount lockAccount,
            IUpdateAccountState updateAccountState, IUnitOfWork unitOfWork)
        {
            _loadAccount = loadAccount ?? throw new ArgumentNullException(nameof(loadAccount));
            _lockAccount = lockAccount ?? throw new ArgumentNullException(nameof(lockAccount));
            _updateAccountState = updateAccountState ?? throw new ArgumentNullException(nameof(updateAccountState));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<bool> SendMoney(SendMoneyCommand command)
        {
            var (sourceAccount, targetAccount) = await LoadAccounts(command.SourceAccountId, command.TargetAccountId);

            await LockAccounts(command.SourceAccountId, command.TargetAccountId);

            sourceAccount.Withdraw(Money.Of(command.Amount));
            targetAccount.Deposit(Money.Of(command.Amount));

            await UpdateAccounts(sourceAccount, targetAccount);
            await UnlockAccounts(command.SourceAccountId, command.TargetAccountId);

            return true;
        }

        private async Task<(Account, Account)> LoadAccounts(long sourceAccountId, long targetAccountId)
        {
            var baselineDate = DateTime.Now.AddDays(-10);

            return (
                await _loadAccount.LoadAccount(AccountId.Of(sourceAccountId), baselineDate),
                await _loadAccount.LoadAccount(AccountId.Of(targetAccountId), baselineDate));
        }

        private async Task LockAccounts(long sourceAccountId, long targetAccountId)
        {
            await _lockAccount.Lock(AccountId.Of(sourceAccountId));
            await _lockAccount.Lock(AccountId.Of(targetAccountId));
            await _unitOfWork.Commit();
        }

        private async Task UpdateAccounts(Account source, Account target)
        {
            await _updateAccountState.Update(source);
            await _updateAccountState.Update(target);
        }

        private async Task UnlockAccounts(long sourceAccountId, long targetAccountId)
        {
            await _lockAccount.Release(AccountId.Of(sourceAccountId));
            await _lockAccount.Release(AccountId.Of(targetAccountId));
            await _unitOfWork.Commit();
        }
    }
}