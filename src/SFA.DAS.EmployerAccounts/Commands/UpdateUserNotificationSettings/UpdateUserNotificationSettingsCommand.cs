﻿using System.Collections.Generic;
using MediatR;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Commands.UpdateUserNotificationSettings
{
    public class UpdateUserNotificationSettingsCommand: IAsyncRequest
    {
        public string UserRef { get; set; }
        public List<UserNotificationSetting> Settings { get; set; }
    }
}
