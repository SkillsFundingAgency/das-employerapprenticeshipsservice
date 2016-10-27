using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.SendNotificationTests
{
    public class WhenSendingNotificationCommand
    {
        private SendNotificationCommandHandler _sendNotificationCommandHandler;
        private Mock<IValidator<SendNotificationCommand>>  _validator;
        private Mock<ILogger> _logger;
        private Mock<INotificationsApi> _notificationClient;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger>();

            _validator = new Mock<IValidator<SendNotificationCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<SendNotificationCommand>())).Returns(new ValidationResult());

            _notificationClient = new Mock<INotificationsApi>();

            _sendNotificationCommandHandler = new SendNotificationCommandHandler(_validator.Object, _logger.Object, _notificationClient.Object);
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
        public async Task ThenTheNotificationClientIsCalled()
        {
            //Arrange
            var sendNotificationCommand = new SendNotificationCommand {Email = new Email {RecipientsAddress = "test@test.com",ReplyToAddress = "noreply@test.com",Subject = "Test Subject", SystemId = "123",TemplateId = "12345",Tokens = new Dictionary<string, string> { {"string","value"} } } };

            //Act
            await _sendNotificationCommandHandler.Handle(sendNotificationCommand);

            //Assert
            _notificationClient.Verify(x=>x.SendEmail(It.Is<Email>(c=>
                        c.RecipientsAddress.Equals(sendNotificationCommand.Email.RecipientsAddress)
                        && c.ReplyToAddress.Equals(sendNotificationCommand.Email.ReplyToAddress)
                        && c.Subject.Equals(sendNotificationCommand.Email.Subject)
                        && c.SystemId.Equals(sendNotificationCommand.Email.SystemId)
                        && c.TemplateId.Equals(sendNotificationCommand.Email.TemplateId)
                        && c.Tokens.Count.Equals(1)
                        )), Times.Once);
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