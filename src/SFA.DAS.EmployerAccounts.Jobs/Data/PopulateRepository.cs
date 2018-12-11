using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerAccounts.Jobs.Data
{
    public class PopulateRepository : BaseRepository, IPopulateRepository
    {
        private readonly EmployerAccountsDbContext _db;

        public PopulateRepository(EmployerAccountsConfiguration configuration, ILog logger, EmployerAccountsDbContext db) 
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task<IEnumerable<MembershipUser>> GetAllAccountUsers()
        {
            var result = await _db.Database.Connection.QueryAsync<MembershipUser>(
                sql: @"SELECT M.UserId, U.UserRef, M.AccountId, M.RoleId FROM [employer_account].[Membership] M  
                            LEFT JOIN [employer_account].[User] U ON M.UserId = U.Id 
                            WHERE M.RoleId IN (1,2,3)", 
                commandType: CommandType.Text);

            return result;
        }

        public async Task<bool> HasJobRun(string job)
        {
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
            var param = new DynamicParameters();
            param.Add("@job", job, DbType.String);

            var result = await _db.Database.Connection.ExecuteAsync("INSERT INTO [dbo].[JobHistory] (Job, Ran) VALUES (@job, GETDATE()) ",
                param: param,
                commandType: CommandType.Text);
        }
    }
}
