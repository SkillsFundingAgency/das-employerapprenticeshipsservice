using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.LevyAccountUpdater.Updater;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.Messaging;


namespace SFA.DAS.EAS.LevyAccountUpdater.UnitTests
{
    public class WhenIUpdateAccounts
    {
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private Mock<IMessagePublisher> _messagePublisher;
        private AccountUpdater _updater;
        private List<Account> _accounts;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Init()
        {
            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _messagePublisher = new Mock<IMessagePublisher>();
            _logger = new Mock<ILogger>();

            _accounts = new List<Account>
            {
                new Account { Id = 1 },
                new Account { Id = 2 }
            };

            _updater = new AccountUpdater(_employerAccountRepository.Object, _messagePublisher.Object, _logger.Object);

            _employerAccountRepository.Setup(x => x.GetAllAccounts()).ReturnsAsync(_accounts);
            _messagePublisher.Setup(x => x.PublishAsync(It.IsAny<EmployerRefreshLevyQueueMessage>())).Returns(Task.Delay(0));
        }

        [Test]
        public async Task ThenAnUpdateAccountMessageShouldBeAddedToTheProcessQueue()
        {
            //Act
            await _updater.RunUpdate();

            //Assert
            _employerAccountRepository.Verify(x => x.GetAllAccounts(), Times.Once);
            _messagePublisher.Verify(x => x.PublishAsync(It.IsAny<EmployerRefreshLevyQueueMessage>()), Times.Exactly(2));
            _messagePublisher.Verify(x => x.PublishAsync(It.Is<EmployerRefreshLevyQueueMessage>(m => m.AccountId.Equals(1))), Times.Once);
            _messagePublisher.Verify(x => x.PublishAsync(It.Is<EmployerRefreshLevyQueueMessage>(m => m.AccountId.Equals(2))), Times.Once);
        }

        [Test]
        public async Task ThenIfNoAccountsAreAvailableTheUpdateShouldNotHappen()
        {
            //Assign
            _employerAccountRepository.Setup(x => x.GetAllAccounts()).ReturnsAsync(new List<Account>());

            //Act
            await _updater.RunUpdate();

            //Assert
            _employerAccountRepository.Verify(x => x.GetAllAccounts(), Times.Once);
            _messagePublisher.Verify(x => x.PublishAsync(It.IsAny<EmployerRefreshLevyQueueMessage>()), Times.Never);
        }

        [Test]
        public void ThenIfAnErrorOccursTheErrorShouldBeLogged()
        {
            //Assign
            var exception = new Exception("Test exception");
            _employerAccountRepository.Setup(x => x.GetAllAccounts()).Throws(exception);
            _logger.Setup(x => x.Error(It.IsAny<Exception>()));

            //Act
            Assert.ThrowsAsync<Exception>(async () => await _updater.RunUpdate());

            //Assert
            _logger.Verify(x => x.Error(exception), Times.Once);
        }
    }
}
