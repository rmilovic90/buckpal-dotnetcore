using System.Threading.Tasks;
using Buckpal.Core.Domain;

namespace Buckpal.Core.Application.Ports.Input
{
    public interface IProvideSendMoneyConfiguration
    {
        Task<Money> GetMaximumAllowedTransactionAmount();
    }
}