using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.EAS.Application.Commands.UnsubscribeNotification;
using SFA.DAS.EAS.Application.Commands.UpdateUserNotificationSettings;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccount;
using SFA.DAS.EAS.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Settings;
using SFA.DAS.EAS.Web.Exceptions;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.NLog.Logger;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class UserSettingsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;
        private readonly ILog _logger;

        //Needed for tests
        protected UserSettingsOrchestrator()
        {
        }

        public UserSettingsOrchestrator(IMediator mediator, IHashingService hashingService, ILog logger)
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

        public async Task<OrchestratorResponse<SummaryUnsubscribeViewModel>> Unsubscribe(
            string userRef, 
            string hashedAccountId, 
            string settingUrl)
        {
            return await CheckUserAuthorization(
                async () =>
                    {
                        var accountId = _hashingService.DecodeValue(hashedAccountId);
                        var settings = await _mediator.SendAsync(new GetUserNotificationSettingsQuery
                        {
                            UserRef = userRef
                        });
                        
                        var userNotificationSettings = settings.NotificationSettings.SingleOrDefault(m => m.AccountId == accountId);

                        if (userNotificationSettings == null)
                            throw new InvalidStateException($"Cannot find user settings for user {userRef} in account {accountId}");

                        if (userNotificationSettings.ReceiveNotifications)
                        {
                            await _mediator.SendAsync(
                                new UnsubscribeNotificationCommand
                                {
                                    UserRef = userRef,
                                    AccountId = accountId,
                                    NotificationSettingUrl = settingUrl
                                });

                            _logger.Info("Unsubscribed from alerts for user {userRef} in account {accountId}");
                        }
                        else {

                            _logger.Info("Already unsubscribed from alerts for user {userRef} in account {accountId}");
                        }

                        return new OrchestratorResponse<SummaryUnsubscribeViewModel>
                        {
                            Data = new SummaryUnsubscribeViewModel
                            {
                                AlreadyUnsubscribed = !userNotificationSettings.ReceiveNotifications,
                                AccountName = userNotificationSettings.Name
                            }
                        };
                    }, hashedAccountId, userRef);
        }

        private void DecodeAccountIds(List<UserNotificationSetting> source)
        {
            foreach (var setting in source)
            {
                setting.AccountId = _hashingService.DecodeValue(setting.HashedAccountId);
            }
        }

        protected async Task<OrchestratorResponse<T>> CheckUserAuthorization<T>(Func<Task<OrchestratorResponse<T>>> code, string hashedAccountId, string externalUserId) where T : class
        {
            try
            {
                await _mediator.SendAsync(new GetEmployerAccountHashedQuery
                {
                    HashedAccountId = hashedAccountId,
                    UserId = externalUserId
                });

                return await code.Invoke();
            }
            catch (UnauthorizedAccessException exception)
            {
                var accountId = _hashingService.DecodeValue(hashedAccountId);
                _logger.Warn($"User not associated to account. UserId:{externalUserId} AccountId:{accountId}");

                return new OrchestratorResponse<T>
                {
                    Status = HttpStatusCode.Unauthorized,
                    Exception = exception
                };
            }
        }
    }
}