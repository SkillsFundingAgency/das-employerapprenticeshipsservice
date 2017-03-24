using SFA.DAS.EAS.Web.Enums;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public sealed class SubmitCommitmenViewModel
    {
        public string HashedAccountId { get; set; }
        public string HashedCommitmentId { get; set; }

        public string Message { get; set; }

        public string LegalEntityCode { get; set; }
        public string LegalEntityName { get; set; }
        public string LegalEntityAddress { get; set; }
        public short LegalEntitySource { get; set; }
        public string ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string CohortRef { get; set; }
        public string SaveOrSend { get; set; }

        public SaveStatus SaveStatus { get; set; }
    }
}