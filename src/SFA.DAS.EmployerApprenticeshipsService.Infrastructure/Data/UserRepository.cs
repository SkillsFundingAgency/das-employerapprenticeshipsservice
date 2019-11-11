using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.Sql.Client;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public UserRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
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