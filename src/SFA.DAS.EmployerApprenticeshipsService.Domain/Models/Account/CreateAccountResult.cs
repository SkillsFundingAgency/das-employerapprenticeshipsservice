namespace SFA.DAS.EAS.Domain.Models.Account
{
    public class CreateAccountResult
    {
        public long AccountId { get; set; }
        public long LegalEntityId { get; set; }
        public long EmployerAgreementId { get; set; }
    }
}
