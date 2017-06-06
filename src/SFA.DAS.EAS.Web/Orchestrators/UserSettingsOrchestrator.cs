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
        private readonly IHashingService _hashingService;
        private readonly ILogger _logger;

        //Needed for tests
        protected UserSettingsOrchestrator()
        {
        }

        public UserSettingsOrchestrator(IMediator mediator, IHashingService hashingService, ILogger logger)
        {
            _mediator = mediator;
            _hashingService = hashingService;
            _logger = logger;
        }

        public virtual async Task<OrchestratorResponse<NotificationSettingsViewModel>> GetNotificationSettingsViewModel(
            string userRef)
        {
            _logger.Info($"Getting user notification settings for user {userRef}");

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
            _logger.Info($"Updating user notification settings for user {userRef}");

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