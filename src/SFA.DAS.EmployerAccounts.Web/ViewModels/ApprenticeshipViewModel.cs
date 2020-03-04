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
        public string ApprenticeshipFullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CourseName { get; set; }
        public DateTime? CourseStartDate { get; set; }
        public DateTime? CourseEndDate { get; set; }
        public ApprenticeshipStatus ApprenticeshipStatus { get; set; }
        public TrainingProviderViewModel TrainingProvider { get; set; }
        public string ViewOrEditApprenticeDetails => $"unapproved/{HashedCohortId}/apprentices/{HashedApprenticeshipId}";
        public string ApprovedOrRejectApprenticeDetails => $"unapproved/{HashedCohortId}";
    }
}