using SFA.DAS.EmployerAccounts.Web.Extensions;
using System;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class Row1Panel1ViewModel
    {
        public int? CohortsCount { get; set; }
        public int? ApprenticeshipsCount { get; set; }
        public int? NumberOfDraftApprentices { get; set; }
        public bool HasDraftApprenticeship { get; set; }
        public string CourseName { get; set; }
        public DateTime? CourseStartDate { get; set; }
        public DateTime? CourseEndDate { get; set; }
        public string ProviderName { get; set; }
        public CohortStatus CohortStatus { get; set; }
        public string HashedDraftApprenticeshipId { get; set; }
        public string HashedCohortReference { get; set; }
        public string ApprenticeName { get; set; }
    }
}