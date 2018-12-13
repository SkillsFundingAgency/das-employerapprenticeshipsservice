using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Data
{
    public class JobHistoryRepository : IJobHistoryRepository
    {
        private readonly ILogger _logger;
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public JobHistoryRepository(Lazy<EmployerAccountsDbContext> db, ILogger logger)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<bool> HasJobRun(string job)
        {
            _logger.LogInformation($"Checking if '{job}' has already run");

            var result = await _db.Value.Database.Connection.QueryAsync<int>(
                sql : @"SELECT 1 FROM [dbo].[JobHistory] 
                            WHERE Job = @Job",
                param :  new {Job = job},
                commandType: CommandType.Text);

            return result.Any();
        }

        public Task MarkJobAsRan(string job)
        {
            _logger.LogInformation($"Marking job '{job}' has as ran");

            return _db.Value.Database.Connection.ExecuteAsync("INSERT INTO [dbo].[JobHistory] (Job, Ran) VALUES (@Job, GETDATE()) ",
                new {Job = job},
                commandType: CommandType.Text);
        }
    }
}