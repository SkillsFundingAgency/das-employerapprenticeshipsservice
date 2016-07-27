using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NLog;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ProcessNotification;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.ProcessNotificationTests
{
    public class WhenIProcessNotificationCommand
    {
        
        private ProcessNotificationCommandHandler _processNotificiationCommandHandler;
        private Mock<IValidator<ProcessNotificationCommand>>  _validator;
        private Mock<INotificationRepository> _notificationRespository;
        private readonly int ExpectedMessageId = 3;
        private readonly int UnExpectedMessageId = 2;
        private readonly int ExpectedUserId = 5;
        private Mock<ILogger> _logger;
        private Mock<IEmailService> _emailService;
        private const string ExpectedReceipientsAddress = "test@local.com";
        private const string ExpectedReplyToAddress = "reply@local.com";

        [SetUp]
        public void Arrange()
        {

            _validator = new Mock<IValidator<ProcessNotificationCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<ProcessNotificationCommand>())).Returns(new ValidationResult());
            
            _notificationRespository = new Mock<INotificationRepository>();
            _notificationRespository.Setup(x => x.GetById(ExpectedMessageId)).ReturnsAsync(new NotificationMessage {
                Data =JsonConvert.SerializeObject(new EmailContent
                    {
                            Data = new Dictionary<string, string> { {"Item", "Item1"} },
                            ReplyToAddress = ExpectedReplyToAddress,
                            RecipientsAddress = ExpectedReceipientsAddress
                    }),DateTime = new DateTime(2015,3,28),ForceFormat = true,MessageFormat = MessageFormat.Email,TemplatedId = "1",UserId = ExpectedUserId});
            _notificationRespository.Setup(x => x.GetById(UnExpectedMessageId)).ReturnsAsync(null);

            _logger = new Mock<ILogger>();

            _emailService = new Mock<IEmailService>();
            
            _processNotificiationCommandHandler = new ProcessNotificationCommandHandler(_validator.Object, _notificationRespository.Object, _logger.Object, _emailService.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Act
            await _processNotificiationCommandHandler.Handle(new ProcessNotificationCommand {Id= ExpectedMessageId});

            //Assert
            _validator.Verify(x=>x.Validate(It.IsAny<ProcessNotificationCommand>()),Times.Once);
        }

        [Test]
        public async Task ThenIfTheMessageIsValidTheNotificationIsRetrievedFromTheRepository()
        {
            //Act
            await _processNotificiationCommandHandler.Handle(new ProcessNotificationCommand {Id= ExpectedMessageId});

            //Assert
            _notificationRespository.Verify(x => x.GetById(ExpectedMessageId), Times.Once);
        }

        [Test]
        public void ThenTheRepositoryIsNotReadIfTheMessageIsInvalidAndAnInvalidRequestExceptionIsThrown()
        {
            //Arrange
            _validator.Setup(x => x.Validate(It.IsAny<ProcessNotificationCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string> { {"",""} } });

            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _processNotificiationCommandHandler.Handle(new ProcessNotificationCommand()));

            //Assert
            _notificationRespository.Verify(x=>x.GetById(It.IsAny<int>()),Times.Never);
            _logger.Verify(x => x.Info("Invalid Request for ProcessNotificationCommand"));
        }

        [Test]
        public void ThenAnInvalidRequestExceptionIsThrownIfTheMessageIsNotFound()
        {
            //Act
            Assert.ThrowsAsync<InvalidRequestException>(async () => await _processNotificiationCommandHandler.Handle(new ProcessNotificationCommand {Id=UnExpectedMessageId}));

            //Assert
            _logger.Verify(x => x.Info($"Notification record not found in repository for id:{UnExpectedMessageId}"));
        }

        [Test]
        public async Task ThenTheEmailServiceIsCalledWhenTheMessageIsValidAndHAsBeenSuccessfullyReturned()
        {
            //Act
            await _processNotificiationCommandHandler.Handle(new ProcessNotificationCommand { Id = ExpectedMessageId });

            //Assert
            _emailService.Verify(x=>x.SendEmail(It.Is<EmailMessage>(c=>c.UserId.Equals(ExpectedUserId) 
                                                                    && c.RecipientsAddress.Equals(ExpectedReceipientsAddress) 
                                                                    && c.ReplyToAddress.Equals(ExpectedReplyToAddress))));
        }
    }
}
