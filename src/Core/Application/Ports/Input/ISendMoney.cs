using System.Threading.Tasks;

namespace Buckpal.Core.Application.Ports.Input
{
    public interface ISendMoney
    {
        Task<bool> SendMoney(SendMoneyCommand command);
    }
}