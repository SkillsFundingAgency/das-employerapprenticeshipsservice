namespace SFA.DAS.EAS.Web.ViewModels.Organisation
{
    public class OrganisationAddedNextStepsViewModel
    {
        public string ErrorMessage { get; set; }
        public string OrganisationName { get; set; }
        public bool ShowWizard { get; set; }
        public string HashedAgreementId { get; set; }
    }
}