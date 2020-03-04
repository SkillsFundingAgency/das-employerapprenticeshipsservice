using SFA.DAS.EmployerAccounts.Models.Commitments;
using System;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class ApprenticeshipViewModel
    {
        public long Id { get; set; }
        public long CohortId { get; set; }
        public string HashedCohortId { get; set; }
        public string HashedApprenticeshipId { get; set; }
        public int? NumberOfDraftApprentices { get; set; }
        public string ApprenticeshipFullName { get; set; }       
        public string CourseName { get; set; }
        public string CourseStartDateText { get; set; }
        public string CourseEndDateText { get; set; }        
        public ApprenticeshipStatus ApprenticeshipStatus { get; set; }
        public TrainingProviderViewModel TrainingProvider { get; set; }
        public bool HasSingleDraftApprenticeship => NumberOfDraftApprentices == 1 && ApprenticeshipStatus == ApprenticeshipStatus.Draft;
        public bool HasApprovedApprenticeship => ApprenticeshipStatus == ApprenticeshipStatus.Approved;
        public string ViewOrEditApprenticeDetails => $"unapproved/{HashedCohortId}/apprentices/{HashedApprenticeshipId}";
        public string ApprovedOrRejectApprenticeDetails => $"unapproved/{HashedCohortId}";
    }
}