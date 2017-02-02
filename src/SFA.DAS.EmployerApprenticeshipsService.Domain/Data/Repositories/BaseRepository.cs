using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public abstract class BaseRepository
    {
        private readonly string _connectionString;

        protected BaseRepository(IConfiguration configuration)
        {
            _connectionString = configuration.DatabaseConnectionString;
        }

        protected async Task<T> WithConnection<T>(Func<IDbConnection, Task<T>> getData)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(); 
                return await getData(connection);
            }
        }
    }
}