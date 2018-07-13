using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Extensions;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.AzureServiceBus.Attributes;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EAS.MessageHandlers.Worker.EventHandlers
{
    [TopicSubscription("MH_RejectedTransferConnectionInvitation")]
    public class RejectedTransferConnectionInvitationEventHandler : MessageProcessor<RejectedTransferConnectionInvitationEvent>
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _employerApprenticeshipsServiceConfiguration;

        public const string UrlFormat = "/accounts/{0}/transfers";

        private readonly EmployerAccountDbContext _db;
        private readonly ILog _log;
        private readonly INotificationsApi _notificationsApi;

        public RejectedTransferConnectionInvitationEventHandler(IMessageSubscriberFactory subscriberFactory, ILog log, EmployerAccountDbContext dbContext, INotificationsApi notificationsApi, EmployerApprenticeshipsServiceConfiguration employerApprenticeshipsServiceConfiguration) : base(subscriberFactory, log)
        {
            _db = dbContext;
            _notificationsApi = notificationsApi;
            _employerApprenticeshipsServiceConfiguration = employerApprenticeshipsServiceConfiguration;
            _log = log;
        }

        protected override async Task ProcessMessage(RejectedTransferConnectionInvitationEvent messageContent)
        {
            IQueryable<User> users;
            try
            {
                users = _db.Users.UsersThatReceiveNotifications(messageContent.SenderAccountId);
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Unable to find users that receive notifications for SenderAccountId '{messageContent.SenderAccountId}'");

                return;
            }

            if (!users.Any())
            {
                _log.Info($"There are no users that receive notifications for SenderAccountId '{messageContent.SenderAccountId}'");
            }

            foreach (var user in users)
            {
                try
                {
                    _log.Info($"Sending email to '{user.Id}' SenderAccountId '{messageContent.SenderAccountId}'");

                    await _notificationsApi.SendEmail(CreateEmail(user, messageContent.ReceiverAccountName,
                        messageContent.SenderAccountHashedId));

                    _log.Info($"Sent email to '{user.Id}' SenderAccountId '{messageContent.SenderAccountId}'");
                }
                catch (Exception ex)
                {
                    _log.Error(ex,
                        $"Unable to send rejected transfer invitation notification to userId {user.Id} for SenderAccountId {messageContent.SenderAccountId} ");
                }
            }
        }

        protected override Task OnErrorAsync(IMessage<RejectedTransferConnectionInvitationEvent> message, Exception ex)
        {
            _log.Error(ex, $"Could not process RejectedTransferConnectionInvitationEvent message for SenderAccountId '{message.Content.SenderAccountId}'");
            return Task.CompletedTask;
        }

        protected override Task OnFatalAsync(Exception ex)
        {
            _log.Fatal(ex, "Failed to process RejectedTransferConnectionInvitationEvent message");
            return Task.CompletedTask;
        }

        private Email CreateEmail(User user, string accountName, string senderExternalId)
        {
            return new Email
            {
                RecipientsAddress = user.Email,
                TemplateId = "TransferConnectionRequestRejected",
                ReplyToAddress = "noreply@sfa.gov.uk",
                Subject = "x",
                SystemId = "x",
                Tokens = new Dictionary<string, string>
                {
                    { "name", user.FirstName },
                    { "account_name", accountName },
                    {
                        "link_notification_page",
                        $"{_employerApprenticeshipsServiceConfiguration.DashboardUrl}{string.Format(UrlFormat, senderExternalId)}"
                    }
                }
            };
        }
    }
}
