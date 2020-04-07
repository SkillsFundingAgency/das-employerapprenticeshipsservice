using System;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models.Account;

namespace SFA.DAS.EmployerAccounts.Models.EmployerAgreement
{
    public class OrganisationAgreement
    {
        /*
        1. [AccountLegalEntity]
        2. [EmployerAgreement]
        3. [LegalEntity]
        4. [EmployerAgreementTemplate]
        */

        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
        public DateTime Created { get; set; }
        public long? SignedAgreementId { get; set; }
        public int? SignedAgreementVersion { get; set; }
        public long? PendingAgreementId { get; set; }
        public int? PendingAgreementVersion { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public DateTime? Deleted { get; set; }

        public virtual ICollection<Account.EmployerAgreement> Agreements { get; set; } = new List<Account.EmployerAgreement>();
        public EmployerAgreementDto EmployerAgreementV1 { get; set; }
        public EmployerAgreementDto EmployerAgreementV2 { get; set; }
        public EmployerAgreementDto EmployerAgreementV3 { get; set; }

        public virtual LegalEntity LegalEntity { get; set; }

        public string HashedAccountId { get; set; }
        public string HashedLegalEntityId { get; set; }
    }
}
