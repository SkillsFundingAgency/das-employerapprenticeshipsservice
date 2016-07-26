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

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.Commands.SendNotificationTests
{
    public class WhenSendingNotificationCommand
    {
        private const int MessageId = 1;
        private SendNotificationCommandHandler _sendNotificationCommandHandler;
        private Mock<IValidator<SendNotificationCommand>>  _validator;
        private Mock<ILogger> _logger;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<INotificationRepository> _notificationRepository;

        [SetUp]
        public void Arrange()
        {
            _logger = new Mock<ILogger>();

            _validator = new Mock<IValidator<SendNotificationCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<SendNotificationCommand>())).Returns(new ValidationResult());

            _messagePublisher = new Mock<IMessagePublisher>();

            _notificationRepository = new Mock<INotificationRepository>();
            _notificationRepository.Setup(x => x.Create(It.IsAny<NotificationMessage>())).ReturnsAsync(MessageId);

            _sendNotificationCommandHandler = new SendNotificationCommandHandler(_validator.Object, _logger.Object, _messagePublisher.Object, _notificationRepository.Object);
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

        [Test]
        public async Task ThenTheNotificationIsWrittenToTheRepostiory()
        {
            //Arrange
            var userId = 1;
            var datetime = new DateTime(2015,01,30);
            var forceFormat = true;
            var templatedId = "678FVR";
            var data = new EmailContent
            {
                Data = new Dictionary<string, string>(),
                RecipientsAddress = "test@local",
                ReplyToAddress = "reply@test"
            };
            var messageFormat = MessageFormat.Email;

            //Act
            await _sendNotificationCommandHandler.Handle(new SendNotificationCommand
            {
                UserId = userId,
                DateTime = datetime,
                ForceFormat = forceFormat,
                TemplatedId = templatedId,
                Data = data,
                MessageFormat = messageFormat
            });

            //Assert
            _notificationRepository.Verify(x=>x.Create(It.Is<NotificationMessage>(
                                                                c=>c.UserId.Equals(userId) 
                                                                    && c.DateTime.Equals(datetime) 
                                                                    && c.ForceFormat.Equals(forceFormat)
                                                                    && c.TemplatedId.Equals(templatedId)
                                                                    && c.Data.Equals(JsonConvert.SerializeObject(data))
                                                                    && c.MessageFormat.Equals(messageFormat)
                                                                    ))
                                                       , Times.Once);
        }

        [Test]
        public async Task ThenTheCommandIsAddedToTheQueue()
        {
            //Act
            await _sendNotificationCommandHandler.Handle(new SendNotificationCommand());

            //Assert
            _messagePublisher.Verify(x=>x.PublishAsync(It.Is<SendNotificationQueueMessage>(c=> c.Id.Equals(MessageId))), Times.Once);
        }
    }
}