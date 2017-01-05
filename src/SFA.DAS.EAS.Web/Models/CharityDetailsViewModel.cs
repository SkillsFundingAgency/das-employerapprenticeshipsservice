namespace SFA.DAS.EAS.Web.Models
{
    public class CharityDetailsViewModel : OrganisationDetailsViewModel
    {
        public string CharityNumber { get; set; }

        public bool IsRemovedError { get; set; }
    }
}