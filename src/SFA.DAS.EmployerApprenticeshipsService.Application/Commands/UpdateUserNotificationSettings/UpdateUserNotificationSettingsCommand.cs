﻿using System;
using System.Collections.Generic;
using MediatR;
using SFA.DAS.EAS.Domain.Models.Settings;

namespace SFA.DAS.EAS.Application.Commands.UpdateUserNotificationSettings
{
    public class UpdateUserNotificationSettingsCommand: IAsyncRequest
    {
        public Guid ExternalUserId { get; set; }
        public List<UserNotificationSetting> Settings { get; set; }
    }
}
