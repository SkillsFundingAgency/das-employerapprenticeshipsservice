using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public abstract class BaseRepository
    {
        private readonly ILogger _logger;
        private readonly string _connectionString;

        protected BaseRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _connectionString = configuration.Employer.DatabaseConnectionString;
        }

        protected async Task<T> WithConnection<T>(Func<IDbConnection, Task<T>> getData)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(); // Asynchronously open a connection to the database
                    return await getData(connection);
                        // Asynchronously execute getData, which has been passed in as a Func<IDBConnection, Task<T>>
                }
            }
            catch (TimeoutException ex)
            {
                _logger.Error(ex);
                throw new Exception($"{GetType().FullName}.WithConnection() experienced a SQL timeout", ex);
            }
            catch (SqlException ex)
            {
                _logger.Error(ex);
                throw new Exception(
                    $"{GetType().FullName}.WithConnection() experienced a SQL exception (not a timeout)", ex);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw new Exception(
                    $"{GetType().FullName}.WithConnection() experienced an exception (not a SQL Exception)", ex);
            }
        }
    }
}