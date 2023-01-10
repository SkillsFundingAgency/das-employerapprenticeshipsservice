using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerAccounts.Data
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public UserRepository(EmployerAccountsConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
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
                sql: "SELECT Id, CONVERT(varchar(64), UserRef) as UserRef, Email, FirstName, LastName, CorrelationId,TermAndConditionsAcceptedOn FROM [employer_account].[User] WHERE UserRef = @userRef",
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
                sql: "SELECT Id, CONVERT(varchar(64), UserRef) as UserRef, Email, FirstName, LastName, CorrelationId FROM [employer_account].[User] WHERE Email = @email",
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
            parameters.Add("@correlationId", user.CorrelationId, DbType.String);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "INSERT INTO [employer_account].[User] (UserRef, Email, FirstName, LastName, CorrelationId) VALUES (@userRef, @email, @firstName, @lastName, @correlationId)",
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

        public Task UpdateTermAndConditionsAcceptedOn(string userRef)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@termAndConditionsAcceptedOn", DateTime.UtcNow, DbType.DateTime);
            parameters.Add("@userRef", new Guid(userRef), DbType.Guid);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "UPDATE [employer_account].[User] set TermAndConditionsAcceptedOn = @termAndConditionsAcceptedOn where UserRef = @userRef",
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
            parameters.Add("@correlationId", user.CorrelationId, DbType.String);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[UpsertUser] @userRef, @email, @firstName, @lastName, @correlationId",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);
        }

        public async Task<Users> GetAllUsers()
        {
            var parameters = new DynamicParameters();

            var result = await _db.Value.Database.Connection.QueryAsync<User>(
                sql: "SELECT Id, CONVERT(varchar(64), UserRef) as UserRef, Email, FirstName, LastName, CorrelationId FROM [employer_account].[User];",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return new Users
            {
                UserList = result.ToList()
            };
        }

        public async Task<IEnumerable<DateTime>> GetAornPayeQueryAttempts(string userRef)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserRef", new Guid(userRef), DbType.Guid);

            var query = await _db.Value.Database.Connection.QueryAsync<DateTime>(
                sql: "[employer_account].[GetUserAornAttempts]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);

            return query.ToList();
        }

        public Task UpdateAornPayeQueryAttempt(string userRef, bool success)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@UserRef", new Guid(userRef), DbType.Guid);
            parameters.Add("@Succeeded", success, DbType.Boolean);

            return _db.Value.Database.Connection.ExecuteAsync(
                sql: "[employer_account].[UpdateUserAornAttempts]",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.StoredProcedure);
        }
    }
}