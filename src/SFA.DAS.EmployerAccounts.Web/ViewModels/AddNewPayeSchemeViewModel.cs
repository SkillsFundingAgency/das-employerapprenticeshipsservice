namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class AddNewPayeSchemeViewModel : ViewModelBase
{
    public string PayeScheme { get; set; }
    public List<LegalEntity> LegalEntities { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string HashedAccountId { get; set; }
    public bool EmprefNotFound { get; set; }
    public string PayeName { get; set; }
}