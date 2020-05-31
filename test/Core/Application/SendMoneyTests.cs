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
        private readonly ILoadAccount _loadAccount = Substitute.For<ILoadAccount>();
        private readonly ILockAccount _lockAccount = Substitute.For<ILockAccount>();
        private readonly IUpdateAccountState _updateAccountState = Substitute.For<IUpdateAccountState>();
        private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

        private readonly ISendMoney _service = ApplicationServicesFactory.CreateSendMoneyService();

        private Account _sourceAccount;
        private Account _targetAccount;

        private bool _sendMoneyResult;

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
            _sourceAccount = AccountOf(id);

            _loadAccount.LoadAccount(
                    Arg.Is(id),
                    Arg.Any<DateTime>())
                .Returns(_sourceAccount);

            return Task.CompletedTask;
        }

        private Task an_existing_target_account(AccountId id, Money balance)
        {
            _targetAccount = AccountOf(id);

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
                await _lockAccount.Lock(_sourceAccount.Id);
                await _updateAccountState.Update(_sourceAccount);
                await _lockAccount.Release(_sourceAccount.Id);

                await _lockAccount.Lock(_targetAccount.Id);
                await _updateAccountState.Update(_targetAccount);
                await _lockAccount.Release(_targetAccount.Id);

                await _unitOfWork.Commit();
            });

            _sourceAccount.Balance.Should().Be(expectedSourceAccountBalance);
            _targetAccount.Balance.Should().Be(expectedTargetAccountBalance);

            return Task.CompletedTask;
        }

        private static Account AccountOf(AccountId id) => new Account(id);

        private static AccountId AccountIdOf(long value) => new AccountId(value);

        private static Money AccountBalanceOf(decimal amount) => MoneyOf(amount);

        private static Money MoneyOf(decimal amount) => new Money(amount);
    }
}