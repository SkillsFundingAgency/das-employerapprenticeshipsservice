using SFA.DAS.EAS.Web.Enums;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public sealed class CommitmentListItemViewModel
    {
        public string HashedCommitmentId { get; set; }
        public string Name { get; set; }
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
        public RequestStatus Status { get; set; }
        public bool ShowViewLink { get; internal set; }
    }
}