using System.Threading.Tasks;
using Buckpal.Core.Domain;

namespace Buckpal.Core.Application.Ports.Output
{
    public interface ILockAccount
    {
        Task Lock(AccountId accountId);
        Task Release(AccountId accountId);
    }
}