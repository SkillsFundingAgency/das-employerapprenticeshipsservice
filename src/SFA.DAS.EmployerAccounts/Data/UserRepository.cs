using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.EmployerAccounts.Models.UserProfile;

namespace SFA.DAS.EmployerAccounts.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public UserRepository(Lazy<EmployerAccountsDbContext> db)
        {
            _db = db;
        }

        public async Task<User> GetUserByRef(string id)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@userRef", new Guid(id), DbType.Guid);

            var result = await _db.Value.Database.Connection.QueryAsync<User>(
                sql:
                "SELECT Id, CONVERT(varchar(64), UserRef) as UserRef, Email, FirstName, LastName FROM [employer_account].[User] WHERE UserRef = @userRef",
                param: parameters,
                transaction: _db.Value.Database.CurrentTransaction.UnderlyingTransaction,
                commandType: CommandType.Text);

            return result.SingleOrDefault();
        }
    }
}