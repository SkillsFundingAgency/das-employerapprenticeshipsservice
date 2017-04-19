using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Infrastructure.Data
{
    public class UserRepository : BaseRepository, IUserRepository
    {

        public UserRepository(EmployerApprenticeshipsServiceConfiguration configuration) : base(configuration)
        {
        }

        public async Task<User> GetUserById(long id)
        {
            var result = await WithConnection(async c =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int64);

                var res = await c.QueryAsync<User>(
                    sql: "SELECT Id, CONVERT(varchar(64), UserRef) as UserRef, Email, FirstName, LastName FROM [employer_account].[User] WHERE Id = @id",
                    param: parameters,
                    commandType: CommandType.Text);
                return res;
            });
            return result.SingleOrDefault();
        }

        public async Task<User> GetByUserRef(string id)
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
