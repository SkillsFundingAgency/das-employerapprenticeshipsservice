using System.Data;
using System.Diagnostics.CodeAnalysis;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;

namespace SFA.DAS.EmployerAccounts.Data;

[ExcludeFromCodeCoverage]
public class EmployerAccountRepository : IEmployerAccountRepository
{
    private readonly Lazy<EmployerAccountsDbContext> _db;

    public EmployerAccountRepository(Lazy<EmployerAccountsDbContext> db)
    {
        _db = db;
    }

    public Task<Account> GetAccountById(long accountId)
    {
        return _db.Value.Accounts.SingleOrDefaultAsync(a => a.Id == accountId);
    }

    public async Task<Accounts<Account>> GetAccounts(string toDate, int pageNumber, int pageSize)
    {
        var offset = pageSize * (pageNumber - 1);

        var countResult = await _db.Value.Database.GetDbConnection().QueryAsync<int>(
            sql: "select count(*) from [employer_account].[Account] a;");

        var result = await _db.Value.Accounts
            .Include(x => x.AccountLegalEntities)
            .ThenInclude(y => y.Agreements)
            .ThenInclude(x=> x.Template)
            .OrderBy(x => x.Id)
            .Skip(offset)
            .Take(pageSize)
            .ToListAsync();

        return new Accounts<Account>
        {
            AccountsCount = countResult.First(),
            AccountList = result
        };
    }

    public async Task<AccountDetail> GetAccountDetailByHashedId(string hashedAccountId)
    {
        var account = await _db.Value.Accounts
            .Include(x => x.AccountLegalEntities)
            .ThenInclude(y => y.Agreements)
            .ThenInclude(x=> x.Template)
            .SingleOrDefaultAsync(x => x.HashedId == hashedAccountId);

        if (account == null)
        {
            return null;
        }

        var accountDetail = new AccountDetail
        {
            AccountId = account.Id,
            HashedId = account.HashedId,
            PublicHashedId = account.PublicHashedId,
            Name = account.Name,
            CreatedDate = account.CreatedDate,
            ApprenticeshipEmployerType = (ApprenticeshipEmployerType)account.ApprenticeshipEmployerType
        };

        var activeLegalEntities = account.AccountLegalEntities.Where(x =>
            x.Deleted == null && x.Agreements.Any(ea =>
                ea.StatusId == EmployerAgreementStatus.Pending ||
                ea.StatusId == EmployerAgreementStatus.Signed));

        accountDetail.LegalEntities = activeLegalEntities.Select(x => x.Id).ToList();
        accountDetail.AccountAgreementTypes = account.AccountLegalEntities.SelectMany(x => x.Agreements).Select(x => x.Template.AgreementType).Distinct().ToList();

        accountDetail.OwnerEmail = account.Memberships
            .Where(m => m.Role == Role.Owner)
            .OrderBy(m => m.CreatedDate)
            .Select(m => m.User.Email)
            .FirstOrDefault();

        accountDetail.PayeSchemes = account.AccountHistory
            .Select(ach => ach.PayeRef)
            .ToList();

        accountDetail.LegalEntities = await _db.Value.AccountLegalEntities
            .Where(ale => ale.AccountId == accountDetail.AccountId
                          && ale.Deleted == null
                          && ale.Agreements.Any(ea =>
                              ea.StatusId == EmployerAgreementStatus.Pending ||
                              ea.StatusId == EmployerAgreementStatus.Signed))
            .Select(ale => ale.LegalEntityId)
            .ToListAsync();

        var templateIds = await _db.Value.Agreements
            .Where(x => accountDetail.LegalEntities.Contains(x.AccountLegalEntity.LegalEntityId) && x.SignedDate.HasValue)
            .Select(x => x.TemplateId)
            .ToListAsync()
            .ConfigureAwait(false);

        accountDetail.AccountAgreementTypes = await _db.Value.AgreementTemplates
            .Where(x => templateIds.Contains(x.Id))
            .Select(x => x.AgreementType)
            .ToListAsync()
            .ConfigureAwait(false);

        return accountDetail;
    }

    public async Task<AccountStats> GetAccountStats(long accountId)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@accountId", accountId, DbType.Int64);

        var result = await _db.Value.Database.GetDbConnection().QueryAsync<AccountStats>(
            sql: "[employer_account].[GetAccountStats]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);

        return result.SingleOrDefault();
    }

    public async Task RenameAccount(long accountId, string name)
    {
        var account = await _db.Value.Accounts.FindAsync(accountId);

        account.Name = name;
        account.ModifiedDate = DateTime.UtcNow;
        account.NameConfirmed = true;
    }

    public Task SetAccountLevyStatus(long accountId, ApprenticeshipEmployerType apprenticeshipEmployerType)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@accountId", accountId, DbType.Int64);
        parameters.Add("@apprenticeshipEmployerType", apprenticeshipEmployerType, DbType.Int16);

        return _db.Value.Database.GetDbConnection().ExecuteAsync(
            sql: "[employer_account].[UpdateAccount_SetAccountApprenticeshipEmployerType]",
            param: parameters,
            transaction: _db.Value.Database.CurrentTransaction?.GetDbTransaction(),
            commandType: CommandType.StoredProcedure);
    }
}