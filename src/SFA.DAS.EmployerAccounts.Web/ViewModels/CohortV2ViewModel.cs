using SFA.DAS.EmployerAccounts.Models.Commitments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    
    public class CohortsV2ViewModel
    {
        public IEnumerable<CohortV2ViewModel> CohortV2WebViewModel { get; set; }
    }

    public class CohortV2ViewModel
    {  
        public long CohortId { get; set; }
        public int? CohortsCount { get; set; }
        public int? NumberOfDraftApprentices { get; set; }
        public string HashedCohortReference { get; set; }
        public string HashedDraftApprenticeshipId { get; set; }
        public CohortStatus CohortStatus { get; set; }
        public virtual ICollection<ApprenticeshipViewModel> Apprenticeships { get; set; } = new List<ApprenticeshipViewModel>();
        public bool HasSingleDraftApprenticeship => CohortsCount == 1 && NumberOfDraftApprentices == 1 &&  Apprenticeships.First().ApprenticeshipStatus == ApprenticeshipStatus.Draft ; //CohortsCount == 1 && NumberOfDraftApprentices == 1 && ApprenticeshipsCount == 0;        
        public bool HasApprovedApprenticeship => Apprenticeships.First().ApprenticeshipStatus == ApprenticeshipStatus.Approved;
        public string ViewOrEditApprenticeDetails => $"unapproved/{HashedCohortReference}/apprentices/{HashedDraftApprenticeshipId}";
        public string ApprovedOrRejectApprenticeDetails => $"unapproved/{HashedCohortReference}";

    }

    public class ApprenticeshipViewModel
    {
        public long Id { get; set; }
        public string ApprenticeshipFullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CourseName { get; set; }
        public DateTime? CourseStartDate { get; set; }
        public DateTime? CourseEndDate { get; set; }
        public ApprenticeshipStatus ApprenticeshipStatus { get; set; }
        public TrainingProviderViewModel TrainingProvider { get; set; }
    }


    public class TrainingProviderViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

}