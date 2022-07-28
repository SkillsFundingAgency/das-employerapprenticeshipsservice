using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class SentTransferConnectionRequestEventNotificationHandler : IHandleMessages<SentTransferConnectionRequestEvent>
    {
        public const string UrlFormat = "/accounts/{0}/transfers";

        private readonly EmployerFinanceConfiguration _config;
        private readonly ILog _logger;
        private readonly INotificationsApi _notificationsApi;
        private readonly Lazy<EmployerFinanceDbContext> _db;

        public SentTransferConnectionRequestEventNotificationHandler(
            EmployerFinanceConfiguration config,
            ILog logger,
            INotificationsApi notificationsApi,
            Lazy<EmployerFinanceDbContext> db)
        {
            _config = config;
            _logger = logger;
            _notificationsApi = notificationsApi;
            _db = db;
        }

        public async Task Handle(SentTransferConnectionRequestEvent message, IMessageHandlerContext context)
        {
            // TO DO: replace with call to outer API which gets the users from Employer Account
            // to which a notification can be sent
            var users = await _db.Value.Users.WhereReceiveNotifications(message.ReceiverAccountId).ToListAsync();

            if (!users.Any())
            {
                _logger.Info($"There are no users that receive notifications for ReceiverAccountId '{message.ReceiverAccountId}'");
            }

            foreach (var user in users)
            {
                try
                {
                    var email = new Email
                    {
                        RecipientsAddress = user.Email,
                        TemplateId = "TransferConnectionInvitationSent",
                        ReplyToAddress = "noreply@sfa.gov.uk",
                        Subject = "x",
                        SystemId = "x",
                        Tokens = new Dictionary<string, string>
                        {
                            { "name", user.FirstName },
                            { "account_name", message.SenderAccountName },
                            {
                                "link_notification_page",
                                $"{_config.DashboardUrl}{string.Format(UrlFormat, message.ReceiverAccountHashedId)}"
                            }
                        }
                    };

                    await _notificationsApi.SendEmail(email);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Unable to send sent transfer request notification to UserId '{user.Id}' for ReceiverAccountId '{message.ReceiverAccountId}'");
                }
            }
        }
    }
}