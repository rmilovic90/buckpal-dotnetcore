using System.Threading.Tasks;
using Buckpal.Core.Domain;

namespace Buckpal.Core.Application.Ports.Output
{
    public interface IUpdateAccountState
    {
        Task Update(Account account);
    }
}