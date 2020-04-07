﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Models.Organisation;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IEmployerAgreementRepository
    {
        [Obsolete("This method has been replaced by the GetAccountEmployerAgreementsQueryHandler query")]
        Task<List<AccountSpecificLegalEntity>> GetLegalEntitiesLinkedToAccount(long accountId, bool signedOnly);
        Task<EmployerAgreementView> GetEmployerAgreement(long agreementId);
        Task SignAgreement(SignEmployerAgreement agreement);
        Task RemoveLegalEntityFromAccount(long agreementId);
        Task<List<RemoveEmployerAgreementView>> GetEmployerAgreementsToRemove(long accountId);
        Task<AccountLegalEntityModel> GetAccountLegalEntity(long accountLegalEntityId);
        Task<IEnumerable<EmployerAgreement>> GetAccountAgreements(long accountId);
        Task<EmployerAgreementStatus?> GetEmployerAgreementStatus(long agreementId);
        Task SetAccountLegalEntityAgreementDetails(long accountLegalEntityId, long? pendingAgreementId, int? pendingAgreementVersion, long? signedAgreementId, int? signedAgreementVersion);

        Task<IEnumerable<OrganisationEmployerAgreementView>> GetAllAgreements(long accountLegalEntityId);

        Task<OrganisationAgreement> GetOrganisationAgreement(long accountLegalEntityId);
    }
}