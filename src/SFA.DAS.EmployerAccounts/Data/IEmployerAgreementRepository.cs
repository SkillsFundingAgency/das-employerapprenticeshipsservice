using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Models.Organisation;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IEmployerAgreementRepository
    {
        Task<List<AccountSpecificLegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId, bool signedOnly);
        Task<EmployerAgreementView> GetEmployerAgreement(long agreementId);
        Task SignAgreement(SignEmployerAgreement agreement);
        Task CreateEmployerAgreementTemplate(string templateRef, string text);
        Task<long> CreateEmployerAgreeement(int templateId, long accountId, long legalEntityId);
        Task<EmployerAgreementTemplate> GetEmployerAgreementTemplate(int templateId);
        Task<EmployerAgreementTemplate> GetLatestAgreementTemplate();

        /// <summary>
        ///     Logically deletes the account legal entity.
        /// </summary>
        /// <returns>An array of the agreements that have been logically removed.</returns>
        Task<EmployerAgreementRemoved[]> RemoveLegalEntityFromAccount(long accountLegalEntityId);
        Task EvaluateEmployerLegalEntityAgreementStatus(long accountId, long legalEntityId);
        Task<AccountLegalEntityModel> GetAccountLegalEntity(long accountLegalEntityId);
    }
}