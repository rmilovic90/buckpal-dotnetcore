using System.Threading.Tasks;
using Buckpal.Core.Application.Ports.Input;

namespace Buckpal.Core.Application.Services
{
    internal sealed class SendMoneyService : ISendMoney
    {
        public Task<bool> SendMoney(SendMoneyCommand command) => Task.FromResult(false);
    }
}