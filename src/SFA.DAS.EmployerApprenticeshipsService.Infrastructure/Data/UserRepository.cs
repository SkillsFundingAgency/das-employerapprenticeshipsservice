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

        public UserRepository(EmployerApprenticeshipsServiceConfiguration configuration, ILog logger, Lazy<EmployerAccountDbContext> db)
            : base(configuration.DatabaseConnectionString, logger)
        {
            _db = db;
        }

        public Task<User> GetUserById(long id)
        {
            return _db.Value.Users.SingleOrDefaultAsync(u => u.Id == id);
        }

        public Task<User> GetUserByRef(Guid @ref)
        {
            return _db.Value.Users.SingleOrDefaultAsync(u => u.Ref == @ref);
        }

        public async Task<User> GetUserByRef(string id)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@userRef", new Guid(id), DbType.Guid);

            var result = await _db.Value.Database.Connection.QueryAsync<User>(
                sql: "SELECT Id, CONVERT(varchar(64), UserRef) as UserRef, Email, FirstName, LastName FROM [employer_account].[User] WHERE UserRef = @userRef",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }

        public async Task<User> GetByEmailAddress(string emailAddress)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@email", emailAddress, DbType.String);

            var result = await _db.Value.Database.Connection.QueryAsync<User>(
                sql: "SELECT Id, CONVERT(varchar(64), UserRef) as UserRef, Email, FirstName, LastName FROM [employer_account].[User] WHERE Email = @email",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }

        public Task Create(User user)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@email", user.Email, DbType.String);
            parameters.Add("@userRef", new Guid(user.UserRef), DbType.Guid);
            parameters.Add("@firstName", user.FirstName, DbType.String);
            parameters.Add("@lastName", user.LastName, DbType.String);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "INSERT INTO [employer_account].[User] (UserRef, Email, FirstName, LastName) VALUES (@userRef, @email, @firstName, @lastName)",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);
        }

        public Task Update(User user)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@email", user.Email, DbType.String);
            parameters.Add("@userRef", new Guid(user.UserRef), DbType.Guid);
            parameters.Add("@firstName", user.FirstName, DbType.String);
            parameters.Add("@lastName", user.LastName, DbType.String);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "UPDATE [employer_account].[User] set Email = @email, FirstName = @firstName, LastName = @lastName where UserRef = @userRef",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);
        }

        public Task Upsert(User user)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@email", user.Email, DbType.String);
            parameters.Add("@userRef", new Guid(user.UserRef), DbType.Guid);
            parameters.Add("@firstName", user.FirstName, DbType.String);
            parameters.Add("@lastName", user.LastName, DbType.String);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[UpsertUser] @userRef, @email, @firstName, @lastName",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);
        }

        public async Task<Users> GetAllUsers()
        {
            var parameters = new DynamicParameters();

            var result = await _db.Value.Database.Connection.QueryAsync<User>(
                sql: "SELECT Id, CONVERT(varchar(64), UserRef) as UserRef, Email, FirstName, LastName FROM [employer_account].[User];",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return new Users
            {
                UserList = result.ToList()
            };
        }
    }
}