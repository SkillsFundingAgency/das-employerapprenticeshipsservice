namespace SFA.DAS.EmployerAccounts.Web.ViewModels;

public class NotificationSettingsViewModel : ViewModelBase
{
    public string HashedId { get; set; }

    public List<UserNotificationSetting> NotificationSettings { get; set; }
    public bool UseGovSignIn { get; set; }
}