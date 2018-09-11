using Dapper;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Models.UserProfile;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Data
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public UserRepository(EmployerAccountsConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public Task<User> GetUserByRef(Guid @ref)
        {
            return _db.Value.Users.SingleOrDefaultAsync(u => u.Ref == @ref);
        }

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