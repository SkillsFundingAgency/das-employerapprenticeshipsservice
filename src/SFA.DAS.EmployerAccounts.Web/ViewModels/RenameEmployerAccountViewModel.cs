namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class RenameEmployerAccountViewModel : ViewModelBase
{
    public string CurrentName { get; set; }
    public string NewName { get; set; }
    public bool?  ChangeAccountName { get; set; }
    public string NewNameError => GetErrorMessage(nameof(NewName));

    public string LegalEntityName { get; internal set; }
}