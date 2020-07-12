using System;
using System.Threading.Tasks;
using Buckpal.Core.Application.Ports.Input;
using Buckpal.Core.Application.Ports.Output;
using Buckpal.Core.Domain;

namespace Buckpal.Core.Application.Services
{
    internal sealed class SendMoneyService : ISendMoney
    {
        private readonly IProvideSendMoneyConfiguration _provideSendMoneyConfiguration;
        private readonly ILoadAccount _loadAccount;
        private readonly ILockAccount _lockAccount;
        private readonly IUpdateAccountState _updateAccountState;
        private readonly IUnitOfWork _unitOfWork;

        public SendMoneyService(IProvideSendMoneyConfiguration provideSendMoneyConfiguration, ILoadAccount loadAccount,
            ILockAccount lockAccount, IUpdateAccountState updateAccountState, IUnitOfWork unitOfWork)
        {
            _provideSendMoneyConfiguration = provideSendMoneyConfiguration;
            _loadAccount = loadAccount;
            _lockAccount = lockAccount;
            _updateAccountState = updateAccountState;
            _unitOfWork = unitOfWork;
        }

        public async Task SendMoney(SendMoneyCommand command)
        {
            await CheckTransactionAmount(Money.Of(command.Amount));

            var (sourceAccount, targetAccount) = await LoadAccounts(command.SourceAccountId, command.TargetAccountId);

            await LockAccounts(command.SourceAccountId, command.TargetAccountId);

            await sourceAccount.Withdraw(
                Money.Of(command.Amount),
                () => UnlockAccounts(command.SourceAccountId, command.TargetAccountId));
            targetAccount.Deposit(Money.Of(command.Amount));

            await UpdateAccounts(sourceAccount, targetAccount);
            await UnlockAccounts(command.SourceAccountId, command.TargetAccountId);
        }

        private async Task CheckTransactionAmount(Money transactionAmount)
        {
            var maximumAllowedTransactionAmount = await _provideSendMoneyConfiguration.GetMaximumAllowedTransactionAmount();
            if (maximumAllowedTransactionAmount < transactionAmount)
                throw new AllowedTransactionAmountOverdrawException(transactionAmount, maximumAllowedTransactionAmount);
        }

        private async Task<(Account, Account)> LoadAccounts(long sourceAccountId, long targetAccountId)
        {
            var baselineDate = DateTime.Now.AddDays(-10);

            return (
                await LoadAccount(sourceAccountId, baselineDate),
                await LoadAccount(targetAccountId, baselineDate)
            );
        }

        private async Task<Account> LoadAccount(long id, DateTime baselineDate) =>
            (await _loadAccount.LoadAccount(AccountId.Of(id), baselineDate))
                ?? throw new AccountNotFoundException(AccountId.Of(id));

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