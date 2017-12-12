using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EAS.Application.Commands.UnsubscribeNotification
{
    public class UnsubscribeNotificationHandler : AsyncRequestHandler<UnsubscribeNotificationCommand>
    {
        private readonly IValidator<UnsubscribeNotificationCommand> _validator;
        private readonly INotificationsApi _notificationsApi;
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ILog _logger;

        public UnsubscribeNotificationHandler(
            IValidator<UnsubscribeNotificationCommand> validator,
            INotificationsApi notificationsApi,
            IUserRepository userRepository,
            IAccountRepository accountRepository,
            ILog logger)
        {
            _validator = validator;
            _notificationsApi = notificationsApi;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _logger = logger;
        }

        protected override async Task HandleCore(UnsubscribeNotificationCommand command)
        {
            _validator.Validate(command);
            
            var settings = await _accountRepository.GetUserAccountSettings(command.UserRef);
            var setting = settings.SingleOrDefault(m => m.AccountId == command.AccountId);
            if (setting == null)
                throw new Exception($"Missing settings for account {command.AccountId} and user with ref {command.UserRef}");
            if(!setting.ReceiveNotifications)
                throw new Exception($"Trying to unsubscribe from an already unsubscribed account, {command.AccountId}");

            setting.ReceiveNotifications = false;
            await _accountRepository.UpdateUserAccountSettings(command.UserRef, settings);

            try
            {
                var user = _userRepository.GetUserByRef(command.UserRef);
                await Task.WhenAll(user);
               _logger.Info($"Sending email to unsubscriber: {user.Result.Id}");
                var email = CreateEmail(user.Result, setting.Name, command.NotificationSettingUrl);
                await _notificationsApi.SendEmail(email);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error sending email to notifications api");
                throw;
            }
        }

        private Email CreateEmail(User user, string accountName, string notificationSettingUrl)
        {
            return new Email
            {
                RecipientsAddress = user.Email,
                TemplateId = "EmployerUnsubscribeAlertSummaryNotification",
                ReplyToAddress = "noreply@sfa.gov.uk",
                Subject = "UnsubscribeSuccessful",
                SystemId = "x",
                Tokens = new Dictionary<string, string>
                        {
                            { "name", user.FirstName },
                            { "account_name", accountName },
                            { "link_notification_page", notificationSettingUrl ?? string.Empty }
                        }
            };
        }
    }
}