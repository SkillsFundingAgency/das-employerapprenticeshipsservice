namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class RenameEmployerAccountViewModel : ViewModelBase
{
    public string HashedId { get; set; }
    public string CurrentName { get; set; }
    public string NewName { get; set; }
    public string NewNameError => GetErrorMessage(nameof(NewName));
}