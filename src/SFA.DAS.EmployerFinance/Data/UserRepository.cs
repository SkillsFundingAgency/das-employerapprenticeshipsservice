using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(EmployerAccountsConfiguration configuration, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        { }

        public Task Upsert(User user)
        {
            return WithConnection(c =>
            {
                var parameters = new DynamicParameters();

                parameters.Add("@email", user.Email, DbType.String);
                parameters.Add("@userRef", new Guid(user.UserRef), DbType.Guid);
                parameters.Add("@firstName", user.FirstName, DbType.String);
                parameters.Add("@lastName", user.LastName, DbType.String);

                return c.ExecuteAsync(
                    sql: "[employer_account].[UpsertUser] @userRef, @email, @firstName, @lastName",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }
    }
}