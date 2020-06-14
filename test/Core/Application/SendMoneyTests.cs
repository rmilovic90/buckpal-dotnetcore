using System;
using System.Threading.Tasks;
using Buckpal.Core.Application.Ports.Input;
using Buckpal.Core.Application.Ports.Output;
using Buckpal.Core.Application.Services;
using Buckpal.Core.Domain;
using FluentAssertions;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using NSubstitute;
using Xunit;

[assembly:LightBddScope]

namespace Buckpal.Core.Application
{
    [Trait("Category", "Acceptance")]
    [FeatureDescription(
        @"In order to transfer money between accounts
        As an user
        I want to send money from one account to another")]
    public sealed class SendMoneyTests : FeatureFixture
    {
        private readonly ILoadAccount _loadAccount;
        private readonly ILockAccount _lockAccount;
        private readonly IUpdateAccountState _updateAccountState;
        private readonly IUnitOfWork _unitOfWork;

        private readonly ISendMoney _service;

        private Account _sourceAccount;
        private Account _targetAccount;

        private Func<Task> _sendMoneyResult;

        public SendMoneyTests()
        {
            _loadAccount = Substitute.For<ILoadAccount>();
            _lockAccount = Substitute.For<ILockAccount>();
            _updateAccountState = Substitute.For<IUpdateAccountState>();
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _service = ApplicationServicesFactory.CreateSendMoneyService(_loadAccount, _lockAccount,
                _updateAccountState, _unitOfWork);
        }

        [Scenario]
        public async Task Transaction_succeeds()
        {
            await Runner.RunScenarioAsync(
                given => an_existing_source_account(
                    AccountId.Of(1L),
                    Money.Of(100M)),
                and => an_existing_target_account(
                    AccountId.Of(2L),
                    Money.Of(150M)),
                when => sending_money(
                    AccountId.Of(1L),
                    AccountId.Of(2L),
                    Money.Of(10.5M)),
                then => the_transaction_is_successful(
                    Money.Of(89.5M),
                    Money.Of(160.5M))
            );
        }

        [Scenario]
        public async Task Transaction_fails_due_to_non_existing_source_account()
        {
            await Runner.RunScenarioAsync(
                given => an_existing_target_account(
                    AccountId.Of(2L),
                    Money.Of(150M)),
                when => sending_money(
                    AccountId.Of(1L),
                    AccountId.Of(2L),
                    Money.Of(10.5M)),
                then => the_transaction_fails_because_of_non_existing_account(AccountId.Of(1L)),
                and => no_interactions_with_source_and_target_accounts_were_performed(),
                and => the_target_account_balance_did_not_change(Money.Of(150M))
            );
        }

        [Scenario]
        public async Task Transaction_fails_due_to_non_existing_target_account()
        {
            await Runner.RunScenarioAsync(
                given => an_existing_source_account(
                    AccountId.Of(1L),
                    Money.Of(100M)),
                when => sending_money(
                    AccountId.Of(1L),
                    AccountId.Of(2L),
                    Money.Of(10.5M)),
                then => the_transaction_fails_because_of_non_existing_account(AccountId.Of(2L)),
                and => no_interactions_with_source_and_target_accounts_were_performed(),
                and => the_source_account_balance_did_not_change(Money.Of(100M))
            );
        }

        [Scenario]
        public async Task Withdrawal_from_source_account_fails_due_to_insufficient_funds()
        {
            await Runner.RunScenarioAsync(
                given => an_existing_source_account(
                    AccountId.Of(1L),
                    Money.Of(100M)),
                and => an_existing_target_account(
                    AccountId.Of(2L),
                    Money.Of(150M)),
                when => sending_money(
                    AccountId.Of(1L),
                    AccountId.Of(2L),
                    Money.Of(150.5M)),
                then => the_transaction_fails_because_of_source_account_insufficient_funds(),
                and => the_source_and_target_account_lock_is_released(),
                and => the_source_and_target_account_balances_did_not_change(
                    Money.Of(100M),
                    Money.Of(150M))
            );
        }

        private Task an_existing_source_account(AccountId id, Money balance)
        {
            _sourceAccount = Account.ExistingOf(id, balance);

            _loadAccount.LoadAccount(
                    Arg.Is(id),
                    Arg.Any<DateTime>())
                .Returns(_sourceAccount);

            return Task.CompletedTask;
        }

        private Task an_existing_target_account(AccountId id, Money balance)
        {
            _targetAccount = Account.ExistingOf(id, balance);

            _loadAccount.LoadAccount(
                    Arg.Is(id),
                    Arg.Any<DateTime>())
                .Returns(_targetAccount);

            return Task.CompletedTask;
        }

        private Task sending_money(AccountId sourceAccountId, AccountId targetAccountId, Money transactionAmount)
        {
            var command = new SendMoneyCommand(
                sourceAccountId,
                targetAccountId,
                transactionAmount);

            _sendMoneyResult = () => _service.SendMoney(command);

            return Task.CompletedTask;
        }

        private async Task the_transaction_is_successful(
            Money expectedSourceAccountBalance, Money expectedTargetAccountBalance)
        {
            await _sendMoneyResult.Should().NotThrowAsync();

            Received.InOrder(async () =>
            {
                var (sourceAccountId, _) = _sourceAccount;
                var (targetAccountId, _) = _targetAccount;

                await _lockAccount.Lock(sourceAccountId);
                await _lockAccount.Lock(targetAccountId);
                await _unitOfWork.Commit();

                await _updateAccountState.Update(_sourceAccount);
                await _updateAccountState.Update(_targetAccount);

                await _lockAccount.Release(sourceAccountId);
                await _lockAccount.Release(targetAccountId);
                await _unitOfWork.Commit();
            });

            var (_, sourceAccountBalance) = _sourceAccount;
            var (_, targetAccountBalance) = _targetAccount;
            sourceAccountBalance.Should().Be(expectedSourceAccountBalance);
            targetAccountBalance.Should().Be(expectedTargetAccountBalance);
        }

        private async Task the_transaction_fails_because_of_non_existing_account(AccountId id)
        {
            (await _sendMoneyResult.Should().ThrowExactlyAsync<AccountNotFoundException>())
                .Which.Id.Should().Be(id);
        }

        private async Task no_interactions_with_source_and_target_accounts_were_performed()
        {
            var (sourceAccountId, _) = _sourceAccount;
            var (targetAccountId, _) = _targetAccount;

            await _lockAccount.DidNotReceive().Lock(sourceAccountId);
            await _updateAccountState.DidNotReceive().Update(_sourceAccount);
            await _lockAccount.DidNotReceive().Release(sourceAccountId);
            await _lockAccount.DidNotReceive().Lock(targetAccountId);
            await _updateAccountState.DidNotReceive().Update(_targetAccount);
            await _lockAccount.DidNotReceive().Release(targetAccountId);
        }

        private Task the_target_account_balance_did_not_change(Money expectedBalance)
        {
            var (_, accountBalance) = _targetAccount;
            accountBalance.Should().Be(expectedBalance);

            return Task.CompletedTask;
        }

        private Task the_source_account_balance_did_not_change(Money expectedBalance)
        {
            var (_, accountBalance) = _sourceAccount;
            accountBalance.Should().Be(expectedBalance);

            return Task.CompletedTask;
        }

        private async Task the_transaction_fails_because_of_source_account_insufficient_funds()
        {
            (await _sendMoneyResult.Should().ThrowExactlyAsync<AccountInsufficientFundsException>())
                .Which.Account.Should().Be(_sourceAccount);
        }

        private Task the_source_and_target_account_lock_is_released()
        {
            Received.InOrder(async () =>
            {
                var (sourceAccountId, _) = _sourceAccount;
                var (targetAccountId, _) = _targetAccount;

                await _lockAccount.Lock(sourceAccountId);
                await _lockAccount.Lock(targetAccountId);
                await _unitOfWork.Commit();

                await _lockAccount.Release(sourceAccountId);
                await _lockAccount.Release(targetAccountId);
                await _unitOfWork.Commit();
            });

            return Task.CompletedTask;
        }

        private async Task the_source_and_target_account_balances_did_not_change(
            Money expectedSourceAccountBalance, Money expectedTargetAccountBalance)
        {
            await _updateAccountState.DidNotReceive().Update(_sourceAccount);
            await _updateAccountState.DidNotReceive().Update(_targetAccount);

            await the_source_account_balance_did_not_change(expectedSourceAccountBalance);
            await the_target_account_balance_did_not_change(expectedTargetAccountBalance);
        }
    }
}