using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

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
    public string ViewOrEditApprenticeDetailsLink => $"unapproved/{HashedCohortId}/apprentices/{HashedApprenticeshipId}";
    public string ApprovedOrRejectApprenticeDetailsLink => $"unapproved/{HashedCohortId}";
    public string ViewApprovedApprenticeDetailsLink => $"apprentices/{HashedApprenticeshipId}/details";        
}