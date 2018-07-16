namespace SFA.DAS.EAS.Account.Api.Types
{
    public class AccountLegalEntityViewModel
    {
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public long LegalEntityId { get; set; }
    }
}