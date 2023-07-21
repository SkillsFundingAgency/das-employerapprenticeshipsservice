namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class GatewayInformViewModel
{
    public string CancelRoute { get; set; }
    public string ConfirmUrl { get; set; }

    public string BreadcrumbUrl { get; set; }
    public string BreadcrumbDescription { get; set; }
    public bool ValidationFailed { get; set; }
}