﻿using System;
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
    [TopicSubscription("MH_SentTransferConnectionInvitiationProcessor")]
    public class SentTransferConnectionInvitationEventHandler : MessageProcessor<SentTransferConnectionInvitationEvent>
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _employerApprenticeshipsServiceConfiguration;

        public const string UrlFormat = "/accounts/{0}/transfers";

        private readonly EmployerAccountDbContext _db;
        private readonly ILog _log;
        private readonly INotificationsApi _notificationsApi;

        public SentTransferConnectionInvitationEventHandler(
            IMessageSubscriberFactory subscriberFactory, 
            ILog log, 
            EmployerAccountDbContext dbContext, 
            INotificationsApi notificationsApi, 
            EmployerApprenticeshipsServiceConfiguration employerApprenticeshipsServiceConfiguration,
            IMessageContextProvider messageContextProvider) 
            : base(subscriberFactory, log, messageContextProvider)
        {
            _db = dbContext;
            _notificationsApi = notificationsApi;
            _employerApprenticeshipsServiceConfiguration = employerApprenticeshipsServiceConfiguration;
            _log = log;
        }

        protected override async Task ProcessMessage(SentTransferConnectionInvitationEvent messageContent)
        {
            IQueryable<User> users;
            try
            {
                users = _db.Users.UsersThatReceiveNotifications(messageContent.ReceiverAccountId);
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Unable to find users that receive notifications for ReceiverAccountId '{messageContent.ReceiverAccountId}'");

                return;
            }

            if (!users.Any())
            {
                _log.Info($"There are no users that receive notifications for ReceiverAccountId '{messageContent.ReceiverAccountId}'");
            }

            foreach (var owner in users)
            {
                try
                {
                    await _notificationsApi.SendEmail(CreateEmail(owner, messageContent.SenderAccountName,
                        messageContent.ReceiverAccountHashedId));
                }
                catch (Exception ex)
                {
                    _log.Error(ex, $"Unable to send sent transfer invitation notification to userId {owner.Id} for Receiver Account Id {messageContent.SenderAccountId} ");
                }
            }
        }

        private Email CreateEmail(User user, string accountName, string senderExternalId)
        {
            return new Email
            {
                RecipientsAddress = user.Email,
                TemplateId = "TransferConnectionInvitationSent",
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
