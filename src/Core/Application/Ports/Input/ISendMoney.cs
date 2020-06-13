using System.Threading.Tasks;

namespace Buckpal.Core.Application.Ports.Input
{
    public interface ISendMoney
    {
        Task SendMoney(SendMoneyCommand command);
    }
}