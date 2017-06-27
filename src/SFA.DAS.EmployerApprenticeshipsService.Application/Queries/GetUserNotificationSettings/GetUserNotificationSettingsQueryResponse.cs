using System.Collections.Generic;
using SFA.DAS.EAS.Domain.Models.Settings;

namespace SFA.DAS.EAS.Application.Queries.GetUserNotificationSettings
{
    public class GetUserNotificationSettingsQueryResponse
    {
        public List<UserNotificationSetting> NotificationSettings { get; set; }
    }
}
