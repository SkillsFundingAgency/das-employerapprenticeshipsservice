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
        private readonly Lazy<EmployerAccountDbContext> _db;

        public UserRepository(EmployerApprenticeshipsServiceConfiguration configuration, Lazy<EmployerAccountDbContext> db, ILog logger)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public Task<User> GetUserById(long id)
        {
            return _db.Value.Users.SingleOrDefaultAsync(u => u.Id == id);
        }

        public Task<User> GetUserByExternalId(Guid externalId)
        {
            return _db.Value.Users.SingleOrDefaultAsync(u => u.ExternalId == externalId);
        }

        public async Task<User> GetUserByRef(string id)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@userRef", new Guid(id), DbType.Guid);

                var res = await c.QueryAsync<User>(
                    sql: "SELECT Id, CONVERT(varchar(64), UserRef) as UserRef, Email, FirstName, LastName FROM [employer_account].[User] WHERE UserRef = @userRef",
                    param: parameters,
                    commandType: CommandType.Text);
                return res;
            });
            return result.SingleOrDefault();
        }

        public async Task<User> GetByEmailAddress(string emailAddress)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", emailAddress, DbType.String);

                return await c.QueryAsync<User>(
                    sql: "SELECT Id, CONVERT(varchar(64), UserRef) as UserRef, Email, FirstName, LastName FROM [employer_account].[User] WHERE Email = @email",
                    param: parameters,
                    commandType: CommandType.Text);
            });
            return result.SingleOrDefault();
        }

        public async Task Create(User user)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", user.Email, DbType.String);
                parameters.Add("@userRef", new Guid(user.UserRef), DbType.Guid);
                parameters.Add("@firstName", user.FirstName, DbType.String);
                parameters.Add("@lastName", user.LastName, DbType.String);
                return await c.ExecuteAsync(
                    sql: "INSERT INTO [employer_account].[User] (UserRef, Email, FirstName, LastName) VALUES (@userRef, @email, @firstName, @lastName)",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task Update(User user)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", user.Email, DbType.String);
                parameters.Add("@userRef", new Guid(user.UserRef), DbType.Guid);
                parameters.Add("@firstName", user.FirstName, DbType.String);
                parameters.Add("@lastName", user.LastName, DbType.String);
                return await c.ExecuteAsync(
                    sql: "UPDATE [employer_account].[User] set Email = @email, FirstName = @firstName, LastName = @lastName where UserRef = @userRef",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task Upsert(User user)
        {
            await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@email", user.Email, DbType.String);
                parameters.Add("@userRef", new Guid(user.UserRef), DbType.Guid);
                parameters.Add("@firstName", user.FirstName, DbType.String);
                parameters.Add("@lastName", user.LastName, DbType.String);
                return await c.ExecuteAsync(
                    sql: "[employer_account].[UpsertUser] @userRef, @email, @firstName, @lastName",
                    param: parameters,
                    commandType: CommandType.Text);
            });
        }

        public async Task<Users> GetAllUsers()
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();

                return await c.QueryAsync<User>(
                    sql: "SELECT Id, CONVERT(varchar(64), UserRef) as UserRef, Email, FirstName, LastName FROM [employer_account].[User];",
                    param: parameters,
                    commandType: CommandType.Text);
            });
            return new Users
            {
                UserList = result.ToList()
            };
        }
    }
}
