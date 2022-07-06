using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.UserProfile;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public UserRepository(EmployerFinanceConfiguration configuration, ILog logger, Lazy<EmployerFinanceDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public async Task Upsert(User user)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@email", user.Email, DbType.String);
            parameters.Add("@userRef", user.Ref, DbType.Guid);
            parameters.Add("@firstName", user.FirstName, DbType.String);
            parameters.Add("@lastName", user.LastName, DbType.String);
            parameters.Add("@correlationId", user.CorrelationId, DbType.String);

            await _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_financial].[UpsertUser] @userRef, @email, @firstName, @lastName, @correlationId",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);
        }
    }
}
