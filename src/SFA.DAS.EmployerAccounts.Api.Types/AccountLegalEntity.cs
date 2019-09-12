namespace SFA.DAS.EmployerAccounts.Api.Types
{
    public class AccountLegalEntity
    {
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public long LegalEntityId { get; set; }
    }
}