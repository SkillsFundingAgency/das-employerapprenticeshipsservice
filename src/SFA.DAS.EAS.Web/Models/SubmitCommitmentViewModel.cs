namespace SFA.DAS.EAS.Web.Models
{
    public sealed class SubmitCommitmentViewModel
    {
        public string HashedAccountId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string LegalEntityCode { get; set; }
        public string LegalEntityName { get; set; }
        public long ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string CohortRef { get; set; }
        public string SaveOrSend { get; set; }
    }
}