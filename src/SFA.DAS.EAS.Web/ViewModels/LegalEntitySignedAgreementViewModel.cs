namespace SFA.DAS.EAS.Web.ViewModels
{
    public class LegalEntitySignedAgreementViewModel
    {
        public string HashedAccountId { get; set; }
        public string LegalEntityCode { get; set; }
        public string CohortRef { get; set; }
        public bool HasSignedAgreement { get; set; }
        public string LegalEntityName { get; set; }
    }
}