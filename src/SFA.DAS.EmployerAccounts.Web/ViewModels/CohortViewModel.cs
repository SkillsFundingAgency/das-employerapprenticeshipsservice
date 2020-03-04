using SFA.DAS.EmployerAccounts.Models.Commitments;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class CohortViewModel
    {  
        public long CohortId { get; set; }        
        public int? NumberOfDraftApprentices { get; set; }
        public string HashedCohortReference { get; set; }        
        public CohortStatus CohortStatus { get; set; }
        public ICollection<ApprenticeshipViewModel> Apprenticeships { get; set; } = new List<ApprenticeshipViewModel>();
        public bool HasSingleDraftApprenticeship =>  NumberOfDraftApprentices == 1 &&  Apprenticeships.Single().ApprenticeshipStatus == ApprenticeshipStatus.Draft ;
        public bool HasApprovedApprenticeship => Apprenticeships?.First().ApprenticeshipStatus == ApprenticeshipStatus.Approved;        
        public string ApprovedOrRejectApprenticeDetails => $"unapproved/{HashedCohortReference}";
    }
}