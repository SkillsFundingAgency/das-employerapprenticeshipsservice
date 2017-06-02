using System.Collections.Generic;
using MediatR;
using SFA.DAS.EAS.Domain.Models.Settings;

namespace SFA.DAS.EAS.Application.Commands.UpdateUserNotificationSettings
{
    public class UpdateUserNotificationSettingsCommand: IAsyncRequest
    {
        public long AccountId { get; set; }
        public string UserRef { get; set; }
        public List<UserNotificationSetting> Settings { get; set; }
    }
}
