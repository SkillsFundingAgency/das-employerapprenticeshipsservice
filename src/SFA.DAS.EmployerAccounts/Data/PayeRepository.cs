using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Data;

public class PayeRepository : IPayeRepository
{
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public PayeRepository(Lazy<EmployerAccountsDbContext> db)
    {
        _db = db;
    }

    public async Task<List<PayeView>> GetPayeSchemesByAccountId(long accountId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@accountId", accountId, DbType.Int64);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<PayeView>(
            sql: "[employer_account].[GetPayeSchemes_ByAccountId]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);

        return result.ToList();
    }

    public Task AddPayeToAccount(Paye payeScheme)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@accountId", payeScheme.AccountId, DbType.Int64);
        parameters.Add("@employerRef", payeScheme.EmpRef, DbType.String);
        parameters.Add("@accessToken", payeScheme.AccessToken, DbType.String);
        parameters.Add("@refreshToken", payeScheme.RefreshToken, DbType.String);
        parameters.Add("@addedDate", DateTime.UtcNow, DbType.DateTime);
        parameters.Add("@employerRefName", payeScheme.RefName, DbType.String);
        parameters.Add("@aorn", payeScheme.Aorn, DbType.String);

        return _db.Value.Database.GetDbConnection().ExecuteAsync(
            sql: "[employer_account].[AddPayeToAccount]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);
    }

    public Task RemovePayeFromAccount(long accountId, string payeRef)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@AccountId", accountId, DbType.Int64);
        parameters.Add("@PayeRef", payeRef, DbType.String);
        parameters.Add("@RemovedDate", DateTime.UtcNow, DbType.DateTime);

        return _db.Value.Database.GetDbConnection().ExecuteAsync(
            sql: "[employer_account].[UpdateAccountHistory]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);
    }

    public async Task<PayeSchemeView> GetPayeForAccountByRef(string hashedAccountId, string reference)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@HashedAccountId", hashedAccountId, DbType.String);
        parameters.Add("@Ref", reference, DbType.String);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<PayeSchemeView>(
            sql: "[employer_account].[GetPayeForAccount_ByRef]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);

        return result.SingleOrDefault();
    }
}