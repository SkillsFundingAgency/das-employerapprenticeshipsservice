using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Data;

public static class AccountLegalEntityExtensions
{
    /// <summary>
    ///     Query extension that selects only legal entities for an account that have either an active signed or active pending agreement).
    /// </summary>
    public static IQueryable<AccountLegalEntity> WithSignedOrPendingAgreementsForAccount(this IQueryable<AccountLegalEntity> query, long accountId)
    {
        return query.Where(ale =>
            ale.AccountId == accountId && ale.Deleted == null && (ale.PendingAgreementId != null || ale.SignedAgreementId != null));
    }
}