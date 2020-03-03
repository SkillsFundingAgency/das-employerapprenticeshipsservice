using SFA.DAS.EmployerAccounts.Models.Commitments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{   
 

    public class CohortV2ViewModel
    {  
        public long CohortId { get; set; }        
        public int? NumberOfDraftApprentices { get; set; }
        public string HashedCohortReference { get; set; }
        public string HashedDraftApprenticeshipId { get; set; }
        public CohortStatus CohortStatus { get; set; }
        public virtual ICollection<ApprenticeshipViewModel> Apprenticeships { get; set; } = new List<ApprenticeshipViewModel>();
        public bool HasSingleDraftApprenticeship =>  NumberOfDraftApprentices == 1 &&  Apprenticeships.Single().ApprenticeshipStatus == ApprenticeshipStatus.Draft ; //CohortsCount == 1 && NumberOfDraftApprentices == 1 && ApprenticeshipsCount == 0;        
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
        public string ViewApprenticeDetails => $"apprentices"; //TODO : View Live ApprenticeLink
    }


    public class TrainingProviderViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

}