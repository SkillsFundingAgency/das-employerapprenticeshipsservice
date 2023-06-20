using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Data;

public class EmployerAccountTeamRepository : IEmployerAccountTeamRepository
{
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public EmployerAccountTeamRepository(Lazy<EmployerAccountsDbContext> db)
    {
        _db = db;
    }

    public async Task<List<TeamMember>> GetAccountTeamMembersForUserId(string hashedAccountId, string externalUserId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@hashedAccountId", hashedAccountId, DbType.String);
        parameters.Add("@externalUserId", Guid.Parse(externalUserId), DbType.Guid);

        const string sql = @"select tm.* from [employer_account].[GetTeamMembers] tm 
                join [employer_account].[Membership] m on m.AccountId = tm.AccountId
                join [employer_account].[User] u on u.Id = m.UserId
                where u.UserRef = @externalUserId and tm.hashedId = @hashedAccountId";

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<TeamMember>(
            sql: sql,
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.Text);

        return result.ToList();
    }

    public async Task<TeamMember> GetMember(string hashedAccountId, string email, bool onlyIfMemberIsActive)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@hashedAccountId", hashedAccountId, DbType.String);
        parameters.Add("@email", email, DbType.String);
        parameters.Add("@onlyIfMemberIsActive", onlyIfMemberIsActive, DbType.Boolean);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<TeamMember>(
            sql: @"SELECT TOP 1 * FROM [employer_account].[GetTeamMembers] " +
                 "WHERE HashedId = @hashedAccountId " +
                 "AND Email = @email " +
                 "AND (@onlyIfMemberIsActive = 0 OR IsUser = 1) " +
                 "ORDER BY IsUser DESC;",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.Text);

        return result.SingleOrDefault();
    }

    public async Task<List<TeamMember>> GetAccountTeamMembers(long accountId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@accountId", accountId, DbType.Int64);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<TeamMember>(
            sql: "[employer_account].[GetEmployerAccountMembers]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);

        return result.ToList();
    }
}