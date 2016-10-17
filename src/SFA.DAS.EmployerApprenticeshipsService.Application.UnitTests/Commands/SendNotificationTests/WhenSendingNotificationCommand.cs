using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.SendNotificationTests
{
    public class WhenSendingNotificationCommand
    {
        private const int MessageId = 1;
        private SendNotificationCommandHandler _sendNotificationCommandHandler;
        private Mock<IValidator<SendNotificationCommand>>  _validator;
        private Mock<ILogger> _logger;
        private Mock<IMessagePublisher> _messagePublisher;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger>();

            _validator = new Mock<IValidator<SendNotificationCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<SendNotificationCommand>())).Returns(new ValidationResult());

            _messagePublisher = new Mock<IMessagePublisher>();
            

            _sendNotificationCommandHandler = new SendNotificationCommandHandler(_validator.Object, _logger.Object, _messagePublisher.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Act
            await _sendNotificationCommandHandler.Handle(new SendNotificationCommand());

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<SendNotificationCommand>()), Times.Once());
        }

        [Test]
        public void ThenAnInvalidRequestExceptionIsThrownAndAnInfoLevelMessageIsLoggedIfTheCommandIsNotValid()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<SendNotificationCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string> {{"", ""}}});

            //Act/Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _sendNotificationCommandHandler.Handle(new SendNotificationCommand()));

            //Assert
            _logger.Verify(x=>x.Info("SendNotificationCommandHandler Invalid Request"), Times.Once);
        }
        
    }
}