using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class NotificationSettingsViewModel : ViewModelBase
    {
        public string HashedId { get; set; }

        public List<UserNotificationSetting> NotificationSettings { get; set; }
    }
}