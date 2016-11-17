using System.Collections.Generic;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EAS.Web.Models
{
    public sealed class CommitmentViewModel
    {
        public string HashedId { get; set; }
        public string Name { get; set; }
        public string LegalEntityName { get; set; }
        public string ProviderName { get; set; }
        public CommitmentStatus Status { get; set; }
        public EditStatus EditStatus { get; set; }

        public IList<ApprenticeshipViewModel> Apprenticeships { get; set; }

    }
}