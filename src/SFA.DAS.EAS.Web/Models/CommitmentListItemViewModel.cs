using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EAS.Web.Models
{
    public sealed class CommitmentListItemViewModel
    {
        public string HashedId { get; set; }
        public string Name { get; set; }
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
        public RequestStatus Status { get; set; }
        public bool ShowViewLink { get; internal set; }
    }
}