using System.Threading.Tasks;

namespace Buckpal.Core.Application.Ports.Output
{
    public interface IUnitOfWork
    {
        Task Commit();
    }
}