using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class CohortViewModel
{        
    public int? NumberOfDraftApprentices { get; set; }
    public string HashedCohortId { get; set; }        
    public CohortStatus CohortStatus { get; set; }
    public ICollection<ApprenticeshipViewModel> Apprenticeships { get; set; } = new List<ApprenticeshipViewModel>();
    public int CohortApprenticeshipsCount => Apprenticeships?.Count ?? 0;        
    public ICollection<TrainingProviderViewModel> TrainingProvider { get; set; } = new List<TrainingProviderViewModel>();
}