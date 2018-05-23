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
    [TopicSubscription("Task_RejectedTransferConnectionInvitation")]
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


            foreach (var owner in users)
            {
                try
                {
                    await _notificationsApi.SendEmail(CreateEmail(owner, messageContent.ReceiverAccountName,
                        messageContent.SenderAccountHashedId));
                }
                catch (Exception ex)
                {
                    _log.Error(ex,
                        $"Unable to send rejected transfer invitation notification to userId {owner.Id} for Receiver Account Id {messageContent.ReceiverAccountId} ");
                }
            }
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
                        $"\"{_employerApprenticeshipsServiceConfiguration.DashboardUrl}{string.Format(UrlFormat, senderExternalId)}\""
                    }
                }
            };
        }
    }
}
