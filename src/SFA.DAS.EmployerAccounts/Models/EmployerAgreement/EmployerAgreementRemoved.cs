namespace SFA.DAS.EmployerAccounts.Models.EmployerAgreement
{
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