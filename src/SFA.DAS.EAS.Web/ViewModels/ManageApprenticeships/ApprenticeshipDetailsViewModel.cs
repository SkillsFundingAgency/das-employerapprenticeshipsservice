using System;

using Microsoft.SqlServer.Server;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    public class ApprenticeshipDetailsViewModel
    {
        public string HashedApprenticeshipId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string TrainingName { get; set; }

        public decimal? Cost { get; set; }

        public string Status { get; set; }

        public string ProviderName { get; set; }

        public PendingChanges PendingChanges { get; set; }
        
        public bool CanEditStatus { get;  set; }

        public string Alert { get; set; }

        public string EmployerReference { get; set; }

        public string CohortReference { get; set; }

        public bool EnableEdit { get; set; }

        public bool HasDataLockError { get; set; }
    }

    public enum PendingChanges
    {
        None = 0,
        ReadyForApproval = 1,
        WaitingForApproval = 2
    }
}