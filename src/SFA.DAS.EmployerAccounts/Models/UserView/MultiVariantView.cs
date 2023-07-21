namespace SFA.DAS.EmployerAccounts.Models.UserView;

public class MultiVariantView
{
    public string Controller { get; set; }
    public string Action { get; set; }
    public bool SplitAccessAcrossUsers { get; set; }
    public List<ViewAccess> Views { get; set; }
}