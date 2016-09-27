using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Models
{
    public sealed class ApproveApprenticeshipViewModel
    {
        public ApprenticeshipViewModel Apprenticeship { get; set; }
        public ApproveApprenticeshipModel ApproveApprenticeshipModel { get; set; }

        public string Name
        {
            get
            {
                return $"{Apprenticeship.FirstName} {Apprenticeship.LastName}";
            }
        }
    }
}