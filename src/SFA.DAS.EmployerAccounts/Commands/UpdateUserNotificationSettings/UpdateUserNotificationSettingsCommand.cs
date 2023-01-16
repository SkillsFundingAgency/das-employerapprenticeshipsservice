using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Commands.UpdateUserNotificationSettings;

public class UpdateUserNotificationSettingsCommand: IRequest
{
    public string UserRef { get; set; }
    public List<UserNotificationSetting> Settings { get; set; }
}