using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SqlConnection _connection;
        private readonly SqlTransaction _transaction;
        private bool _committed;
        
        public UnitOfWork(SqlConnection connection)
        {
            _connection = connection;
            _transaction = connection.BeginTransaction(IsolationLevel.ReadUncommitted);
        }
        //Handle calling these twice
        public void CommitChanges()
        {
            _committed = true;
            _transaction.Commit();
        }
        //Handle calling these twice
        public void RollbackChanges()
        {
            _transaction.Rollback();
        }

        public async Task Execute(string command, object param = null, CommandType? type = null)
        {
            await _transaction.Connection.ExecuteAsync(command, param, _transaction, null, type);
        }

        public void Dispose()
        {
            if (!_committed)
            {
                RollbackChanges();
            }

            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }
}

