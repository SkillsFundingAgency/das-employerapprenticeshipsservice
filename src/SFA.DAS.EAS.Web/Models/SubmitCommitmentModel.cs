namespace SFA.DAS.EAS.Web.Models
{
    using SFA.DAS.EAS.Web.Models.Types;

    public sealed class SubmitCommitmentModel
    {
        public string HashedAccountId { get; set; }
        public string HashedCommitmentId { get; set; }
        public string Message { get; set; }
        public string LegalEntityCode { get; set; }
        public string LegalEntityName { get; set; }
        public string ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string CohortRef { get; set; }
        public string SaveOrSend { get; set; }

        public SaveStatus SaveStatus { get; set; }
    }
}