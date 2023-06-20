using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserNotificationSettings;

public class GetUserNotificationSettingsQueryResponse
{
    public List<UserNotificationSetting> NotificationSettings { get; set; }
}