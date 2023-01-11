namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class OrganisationAddedNextStepsViewModel
{
    public string ErrorMessage { get; set; }
    public string OrganisationName { get; set; }
    public bool ShowWizard { get; set; }
    public string HashedAgreementId { get; set; }
}