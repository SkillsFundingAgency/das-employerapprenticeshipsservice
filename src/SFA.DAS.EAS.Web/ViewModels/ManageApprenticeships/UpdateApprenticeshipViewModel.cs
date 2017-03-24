using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    public class UpdateApprenticeshipViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmployerRef { get; set; }

        public Apprenticeship OriginalApprenticeship { get; set; }

        public decimal? Cost { get; set; }

        public DateTimeViewModel StartDate { get; set; }

        public DateTimeViewModel EndDate { get; set; }

        public DateTimeViewModel DateOfBirth { get; set; }

        public TrainingType? TrainingType { get; set; }

        public string TrainingCode { get; set; }

        public string TrainingName { get; set; }
    }
}