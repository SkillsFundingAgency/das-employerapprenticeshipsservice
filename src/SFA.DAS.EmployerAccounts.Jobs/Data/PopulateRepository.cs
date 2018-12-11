using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Data;

namespace SFA.DAS.EmployerAccounts.Jobs.Data
{
    public class PopulateRepository : IPopulateRepository
    {
        private readonly ILogger<PopulateRepository> _logger;
        private readonly EmployerAccountsDbContext _db;

        public PopulateRepository(Lazy<EmployerAccountsDbContext> db, ILogger<PopulateRepository> logger)
        {
            _logger = logger;
            _db = db.Value;
        }

        public async Task<IEnumerable<MembershipUser>> GetAllAccountUsers()
        {
            _logger.LogInformation("Getting All Users to be migrated");

            var result = await _db.Database.Connection.QueryAsync<MembershipUser>(
                sql: @"SELECT M.UserId, U.UserRef, M.AccountId, M.RoleId FROM [employer_account].[Membership] M  
                            LEFT JOIN [employer_account].[User] U ON M.UserId = U.Id 
                            WHERE M.RoleId IN (1,2,3)", 
                commandType: CommandType.Text);

            return result;
        }

        public async Task<bool> HasJobRun(string job)
        {
            _logger.LogInformation($"Checking if '{job}' has already run");

            var param = new DynamicParameters();
            param.Add("@job", job, DbType.String);

            var result = await _db.Database.Connection.QueryAsync<int>(
                sql: @"SELECT 1 FROM [dbo].[JobHistory] 
                            WHERE Job = @job",
                param: param,
                commandType: CommandType.Text);

            return result.Any();
        }

        public async Task MarkJobAsRan(string job)
        {
            _logger.LogInformation($"Marking job '{job}' has as ran");

            var param = new DynamicParameters();
            param.Add("@job", job, DbType.String);

            var result = await _db.Database.Connection.ExecuteAsync("INSERT INTO [dbo].[JobHistory] (Job, Ran) VALUES (@job, GETDATE()) ",
                param: param,
                commandType: CommandType.Text);
        }
    }
}
