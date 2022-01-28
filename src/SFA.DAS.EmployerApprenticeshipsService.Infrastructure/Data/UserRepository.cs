using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public UserRepository(Lazy<EmployerAccountsDbContext> db)
        {
            _db = db;
        }

        public Task Upsert(User user)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@email", user.Email, DbType.String);
            parameters.Add("@userRef", new Guid(user.UserRef), DbType.Guid);
            parameters.Add("@firstName", user.FirstName, DbType.String);
            parameters.Add("@lastName", user.LastName, DbType.String);
            parameters.Add("@correlationId", null, DbType.String);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[UpsertUser] @userRef, @email, @firstName, @lastName, @correlationId",
                param: parameters,
                commandType: CommandType.Text,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction);
        }
    }
}