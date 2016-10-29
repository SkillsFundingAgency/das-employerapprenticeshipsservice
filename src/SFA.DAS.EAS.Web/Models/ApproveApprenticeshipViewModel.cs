namespace SFA.DAS.EAS.Web.Models
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