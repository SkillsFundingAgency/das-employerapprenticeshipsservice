using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SFA.DAS.EAS.Domain.Models.Settings;

namespace SFA.DAS.EAS.Web.ViewModels
{
    public class NotificationSettingsViewModel : ViewModelBase
    {
        public string HashedId { get; set; }

        public List<UserNotificationSetting> NotificationSettings { get; set; }

    }
}