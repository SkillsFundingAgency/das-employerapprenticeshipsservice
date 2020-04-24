﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using NServiceBus;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Messages.Commands;

namespace SFA.DAS.EmployerAccounts.Commands.ReportTrainingProvider
{
    public class ReportTrainingProviderCommandHandler : AsyncRequestHandler<ReportTrainingProviderCommand>
    {
        private const string ReportTrainingProviderTemplateId = "ReportTrainingProviderNotification";
        private readonly EmployerAccountsConfiguration _configuration;
        private readonly IMessageSession _publisher;
        private readonly ILog _logger;

        public ReportTrainingProviderCommandHandler(
            IMessageSession publisher,
            ILog logger,
            EmployerAccountsConfiguration configuration)
        {
            _publisher = publisher;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task HandleCore(ReportTrainingProviderCommand message)
        {
            var tokens = new Dictionary<string, string>()
            {
                {"employer_email", message.EmployerEmailAddress },
                {"email_reported_date", message.EmailReportedDate.ToString("g") },
                {"training_provider", message.TrainingProvider },
                {"training_provider_name", message.TrainingProviderName },
                {"invitation_email_sent_date", message.InvitationEmailSentDate.ToString("g") },
             };

            if (string.IsNullOrWhiteSpace(_configuration.ReportTrainingProviderEmailAddress))
            {
                var exception = new ArgumentNullException("reportTrainingProviderEmailAddress", "ReportTrainingProviderEmailAddress configuration value can not be blank.");
                _logger.Error(exception, "Report Training Provider Email must be provided in configuration");
                throw exception;
            }

            await _publisher.Send(new SendEmailCommand(ReportTrainingProviderTemplateId, _configuration.ReportTrainingProviderEmailAddress, tokens));

        }
    }
}