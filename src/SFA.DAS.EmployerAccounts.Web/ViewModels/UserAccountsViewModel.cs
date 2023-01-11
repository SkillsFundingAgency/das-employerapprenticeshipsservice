namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class UserAccountsViewModel
{
    public Accounts<Account> Accounts;
    public int Invitations;
    public FlashMessageViewModel FlashMessage;
    public string ErrorMessage;
    public DateTime? TermAndConditionsAcceptedOn { get; set; }
    public DateTime? LastTermsAndConditionsUpdate { get; set; }
    public bool ShowTermsAndConditionBanner { get 
        { 
            if (LastTermsAndConditionsUpdate.HasValue)
            {
                if (!TermAndConditionsAcceptedOn.HasValue ||  
                    (TermAndConditionsAcceptedOn.Value < LastTermsAndConditionsUpdate.Value))
                {
                    return true;
                }
            }

            return false;
        } 
    }
}