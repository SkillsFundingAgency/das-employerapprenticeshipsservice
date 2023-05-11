namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class UserAccountsViewModel
{
    public Accounts<Account> Accounts { get; set; }
    public int Invitations { get; set; }
    public FlashMessageViewModel FlashMessage { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime? TermAndConditionsAcceptedOn { get; set; }
    public DateTime? LastTermsAndConditionsUpdate { get; set; }

    public bool ShowTermsAndConditionBanner { get 
        {
            if (!LastTermsAndConditionsUpdate.HasValue)
            {
                return false;
            }

            return !TermAndConditionsAcceptedOn.HasValue || TermAndConditionsAcceptedOn.Value < LastTermsAndConditionsUpdate.Value;
        } 
    }
}