using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Application.Commands.UpdateUserNotificationSettings;
using SFA.DAS.EAS.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Settings;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class UserSettingsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        private readonly IHashingService _hashingService;

        //Needed for tests
        public UserSettingsOrchestrator()
        {
        }

        public UserSettingsOrchestrator(IMediator mediator, ILogger logger, IHashingService hashingService)
        {
            _mediator = mediator;
            _logger = logger;
            _hashingService = hashingService;
        }

        public virtual async Task<OrchestratorResponse<NotificationSettingsViewModel>> GetNotificationSettingsViewModel(
            string userRef)
        {
            var response = await _mediator.SendAsync(new GetUserNotificationSettingsQuery
            {
                UserRef = userRef
            });

            return new OrchestratorResponse<NotificationSettingsViewModel>
            {
                Data = new NotificationSettingsViewModel
                {
                    HashedId = userRef,
                    NotificationSettings = response.NotificationSettings
                },
            };
        }

        public virtual async Task UpdateNotificationSettings(
            string userRef, List<UserNotificationSetting> settings)
        {
            DecodeAccountIds(settings);

            await _mediator.SendAsync(new UpdateUserNotificationSettingsCommand
            {
                UserRef = userRef,
                Settings = settings
            });
        }

        private void DecodeAccountIds(List<UserNotificationSetting> source)
        {
            foreach (var setting in source)
            {
                setting.AccountId = _hashingService.DecodeValue(setting.HashedAccountId);
            }
        }
    }
}