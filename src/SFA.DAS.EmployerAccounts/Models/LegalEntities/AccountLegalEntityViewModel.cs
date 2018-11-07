namespace SFA.DAS.EmployerAccounts.Models.LegalEntities
{
    public class AccountLegalEntityViewModel
    {
        public long AccountId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        public long LegalEntityId { get; set; }
    }
}