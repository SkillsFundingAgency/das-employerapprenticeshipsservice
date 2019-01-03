using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EmployerAccounts.Models.EmployerAgreement
{
    public class RemoveOrganisationView
    {
        public string Name { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public string HashedAccountId { get; set; }
        public bool CanBeRemoved { get; set; }
        public string LegalEntityCode { get; set; }
        public OrganisationType LegalEntitySource { get; set; }
        public bool HasSignedAgreement { get; set; }
    }


    public class EmployerAgreementRemoved
    {
        public long EmployerAgreementId { get; set; }
        public long LegalEntityId { get; set; } 
        public string LegalEntityName { get; set; }
        public long AccountLegalEntityId { get; set; }
        public EmployerAgreementStatus PreviousStatus { get; set; }
        public bool Signed { get; set; }
    }
}