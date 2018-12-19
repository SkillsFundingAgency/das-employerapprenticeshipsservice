using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.Jobs.Data
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly ILogger _logger;
        private readonly Lazy<EmployerAccountsDbContext> _db;

        public MembershipRepository(Lazy<EmployerAccountsDbContext> db, ILogger logger)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IEnumerable<MembershipUser>> GetAllAccountUsers()
        {
            _logger.LogInformation("Getting All Users to be migrated");

            var result = await _db.Value.Database.Connection.QueryAsync<MembershipUser>(
                @"SELECT M.UserId, U.UserRef, M.AccountId, M.RoleId FROM [employer_account].[Membership] M  
                            JOIN [employer_account].[User] U ON M.UserId = U.Id 
                            WHERE M.RoleId IN @Roles",
                new { Roles = new HashSet<UserRole> { UserRole.Owner, UserRole.Transactor, UserRole.Transactor } },
                commandType: CommandType.Text);

            return result;
        }
    }
}