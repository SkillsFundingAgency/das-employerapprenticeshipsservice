using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.EmployerAccounts.Data;

public class EmployerSchemesRepository : BaseRepository, IEmployerSchemesRepository
{
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public EmployerSchemesRepository(EmployerAccountsConfiguration configuration, ILog logger, Lazy<EmployerAccountsDbContext> db)
        : base(configuration.DatabaseConnectionString, logger)
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
            commandType: CommandType.StoredProcedure);

        return result.SingleOrDefault();
    }
}