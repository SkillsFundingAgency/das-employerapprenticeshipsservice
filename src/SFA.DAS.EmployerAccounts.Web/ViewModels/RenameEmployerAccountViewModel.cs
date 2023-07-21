namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class RenameEmployerAccountViewModel : ViewModelBase
{
    public string CurrentName { get; set; }
    private string _newName;
    public string NewName
    {
        get
        {
            return ChangeAccountName.GetValueOrDefault() ? _newName : CurrentName;
        }
        set
        {
            _newName = value;
        }
    }
    public bool?  ChangeAccountName { get; set; }
    public string NewNameError => GetErrorMessage(nameof(NewName));

    public string LegalEntityName { get; set; }
}