using SFA.DAS.EmployerAccounts.Models.CommitmentsV2;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class CohortViewModel
    {        
        public int? NumberOfDraftApprentices { get; set; }
        public string HashedCohortId { get; set; }        
        public CohortStatus CohortStatus { get; set; }
        public ICollection<ApprenticeshipViewModel> Apprenticeships { get; set; } = new List<ApprenticeshipViewModel>();                      
    }
}