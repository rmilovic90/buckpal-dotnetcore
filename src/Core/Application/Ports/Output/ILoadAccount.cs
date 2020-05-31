using System;
using System.Threading.Tasks;
using Buckpal.Core.Domain;

namespace Buckpal.Core.Application.Ports.Output
{
    public interface ILoadAccount
    {
        Task<Account> LoadAccount(AccountId id, DateTime baselineDate);
    }
}