using System;
using System.Data;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IUnitOfWork : IDisposable
    {
        void CommitChanges();
        void RollbackChanges();
        Task Execute(string command, object param = null, CommandType? type = null);
    }
}