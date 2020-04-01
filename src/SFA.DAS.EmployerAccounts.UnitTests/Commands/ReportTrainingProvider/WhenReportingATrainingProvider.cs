﻿using MediatR;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.ReportTrainingProvider;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.ReportTrainingProvider
{
    public class WhenReportingATrainingProvider
    {
        private ReportTrainingProviderCommandHandler _handler;
        private Mock<ILog> _logger;
        private Mock<IMessageSession> _publisher;
        private EmployerAccountsConfiguration _configuration;
        private ReportTrainingProviderCommand _validCommand;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILog>();
            _publisher = new Mock<IMessageSession>();
            _configuration = new EmployerAccountsConfiguration
            {
                ReportTrainingProviderEmailAddress = "Report@TrainingProvider.com"
            };

            _handler = new ReportTrainingProviderCommandHandler(_publisher.Object, _logger.Object, _configuration);

            _validCommand = new ReportTrainingProviderCommand("EmployerEmail@dfe.com", DateTime.Now, "TrainingProvider", "TrainingProviderName", DateTime.Now.AddHours(-1));

        }

        [Test]
        public void ThenAnExceptionIsThrownIfThereIsNoEmailAddressInTheConfiguration()
        {
            //Arrange
            _configuration.ReportTrainingProviderEmailAddress = "";

            //Act Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(_validCommand));
        }

        [Test]
        public void ThenAnEmailCommandIsSentWhenAValidCommandIsReceived()
        {
            //Arrange
            var tokens = new Dictionary<string, string>()
            {
                {"employer_email", _validCommand.EmployerEmailAddress },
                {"email_reported_date", _validCommand.EmailReportedDate.ToString("g") },
                {"training_provider", _validCommand.TrainingProvider },
                {"training_provider_name", _validCommand.TrainingProviderName },
                {"invitation_email_sent_date", _validCommand.InvitationEmailSentDate.ToString("g") },
             };

            //Act 
            _handler.Handle(_validCommand);
            
            //Assert
            _publisher.Verify(s => s.Send(It.Is<SendEmailCommand>(x => 
                x.Tokens.OrderBy(u => u.Key).SequenceEqual(tokens.OrderBy(t => t.Key))
                &&
                x.RecipientsAddress == _configuration.ReportTrainingProviderEmailAddress
                && 
                x.TemplateId == "ReportTrainingProviderNotification"
            ), It.IsAny<SendOptions>()));
        }

    }
}
