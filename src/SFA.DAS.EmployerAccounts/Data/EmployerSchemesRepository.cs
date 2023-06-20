using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Data;

public class EmployerSchemesRepository : IEmployerSchemesRepository
{
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public EmployerSchemesRepository(Lazy<EmployerAccountsDbContext> db)
    {
        _db = db;
    }

    public async Task<PayeSchemes> GetSchemesByEmployerId(long employerId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@accountId", employerId, DbType.Int64);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<PayeScheme>(
            sql: "[employer_account].[GetPayeSchemes_ByAccountId]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);

        return new PayeSchemes
        {
            SchemesList = result.ToList()
        };
    }

    public async Task<PayeScheme> GetSchemeByRef(string empref)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@payeRef", empref, DbType.String);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<PayeScheme>(
            sql: "[employer_account].[GetPayeSchemesInUse]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);

        return result.SingleOrDefault();
    }
}