namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class ConfirmNewPayeSchemeViewModel : AddNewPayeSchemeViewModel
{
    public ConfirmNewPayeSchemeViewModel()
    {
    }

    public ConfirmNewPayeSchemeViewModel(AddNewPayeSchemeViewModel model)
    {
        AccessToken = model.AccessToken;
        RefreshToken = model.RefreshToken;
        PayeScheme = model.PayeScheme;
        HashedAccountId = model.HashedAccountId;
    }
}