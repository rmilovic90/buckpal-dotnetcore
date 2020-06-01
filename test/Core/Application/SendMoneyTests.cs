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

        private bool _sendMoneyResult;

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
        public async Task Transaction_succeeds_when_accounts_exist_and_have_sufficient_balances()
        {
            await Runner.RunScenarioAsync(
                given => an_existing_source_account(
                    AccountIdOf(1L),
                    AccountBalanceOf(100M)),
                and => an_existing_target_account(
                    AccountIdOf(2L),
                    AccountBalanceOf(150M)),
                when => sending_money(
                    AccountIdOf(1L),
                    AccountIdOf(2L),
                    MoneyOf(10.5M)),
                then => the_transaction_is_successful(
                    AccountBalanceOf(89.5M),
                    AccountBalanceOf(160.5M)));
        }

        private Task an_existing_source_account(AccountId id, Money balance)
        {
            _sourceAccount = AccountOf(id, balance);

            _loadAccount.LoadAccount(
                    Arg.Is(id),
                    Arg.Any<DateTime>())
                .Returns(_sourceAccount);

            return Task.CompletedTask;
        }

        private Task an_existing_target_account(AccountId id, Money balance)
        {
            _targetAccount = AccountOf(id, balance);

            _loadAccount.LoadAccount(
                    Arg.Is(id),
                    Arg.Any<DateTime>())
                .Returns(_targetAccount);

            return Task.CompletedTask;
        }

        private async Task sending_money(AccountId sourceAccountId, AccountId targetAccountId, Money transactionAmount)
        {
            var command = new SendMoneyCommand(
                sourceAccountId,
                targetAccountId,
                transactionAmount);

            _sendMoneyResult = await _service.SendMoney(command);
        }

        private Task the_transaction_is_successful(Money expectedSourceAccountBalance, Money expectedTargetAccountBalance)
        {
            _sendMoneyResult.Should().BeTrue();

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

            return Task.CompletedTask;
        }

        private static Account AccountOf(AccountId id, Money balance) => new Account(id, balance);

        private static AccountId AccountIdOf(long value) => new AccountId(value);

        private static Money AccountBalanceOf(decimal amount) => MoneyOf(amount);

        private static Money MoneyOf(decimal amount) => new Money(amount);
    }
}