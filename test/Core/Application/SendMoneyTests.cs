using Buckpal.Core.Application.Ports.Input;
using Buckpal.Core.Application.Services;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
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
        private const decimal SampleAmount = 10.50M;

        private readonly ISendMoney _service = ApplicationServicesFactory.CreateSendMoneyService();

        [Scenario]
        public void Transaction_succeeds()
        {
            Runner.RunScenario(
                given => a_valid_source_account(),
                and => a_valid_target_account(),
                and => withdrawal_from_source_account_is_allowed(),
                and => deposit_to_target_account_is_allowed(),
                when => sending_money(SampleAmount),
                then => the_transaction_is_successful());
        }

        private void the_transaction_is_successful()
        {
            throw new System.NotImplementedException();
        }

        private void a_valid_source_account()
        {
            throw new System.NotImplementedException();
        }

        private void a_valid_target_account()
        {
            throw new System.NotImplementedException();
        }

        private void withdrawal_from_source_account_is_allowed()
        {
            throw new System.NotImplementedException();
        }

        private void deposit_to_target_account_is_allowed()
        {
            throw new System.NotImplementedException();
        }

        private void sending_money(decimal amount)
        {
            throw new System.NotImplementedException();
        }
    }
}