using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiRequests.Projections;
using SFA.DAS.EmployerFinance.Infrastructure.OuterApiResponses.Accounts;
using SFA.DAS.EmployerFinance.Interfaces.OuterApi;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerFinance.MessageHandlers.EventHandlers
{
    public class ApprovedTransferConnectionRequestEventNotificationHandler : IHandleMessages<ApprovedTransferConnectionRequestEvent>
    {
        public const string UrlFormat = "/accounts/{0}/transfers/connections";

        private readonly EmployerFinanceConfiguration _config;
        private readonly IOuterApiClient _outerApiClient;
        private readonly ILog _logger;
        private readonly INotificationsApi _notificationsApi;

        public ApprovedTransferConnectionRequestEventNotificationHandler(
            EmployerFinanceConfiguration config,
            IOuterApiClient outerApiClient,
            ILog logger,
            INotificationsApi notificationsApi)
        {
            _config = config;
            _outerApiClient = outerApiClient;
            _logger = logger;
            _notificationsApi = notificationsApi;
        }

        public async Task Handle(ApprovedTransferConnectionRequestEvent message, IMessageHandlerContext context)
        {
            var users = await _outerApiClient.Get<GetAccountTeamMembersWhichReceiveNotificationsResponse>(
                new GetAccountTeamMembersWhichReceiveNotificationsRequest(message.SenderAccountId));

            if (!users.Any())
            {
                _logger.Info($"There are no users that receive notifications for SenderAccountId '{message.SenderAccountId}'");
            }

            foreach (var user in users)
            {
                try
                {
                    var email = new Email
                    {
                        RecipientsAddress = user.Email,
                        TemplateId = "TransferConnectionRequestApproved",
                        ReplyToAddress = "noreply@sfa.gov.uk",
                        Subject = "x",
                        SystemId = "x",
                        Tokens = new Dictionary<string, string>
                        {
                            {"name", user.FirstName},
                            {"account_name", message.ReceiverAccountName},
                            {
                                "link_notification_page",
                                $"{_config.EmployerFinanceBaseUrl}{string.Format(UrlFormat, message.SenderAccountHashedId)}"
                            }
                        }
                    };

                    await _notificationsApi.SendEmail(email);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Unable to send approved transfer request notification to UserRef '{user.UserRef}' for SenderAccountId '{message.SenderAccountId}'");
                }
            }
        }
    }
}