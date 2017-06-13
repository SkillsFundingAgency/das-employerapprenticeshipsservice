using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Domain.Data.Entities.Account;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.PAYE;
using SFA.DAS.EAS.LevyAccountUpdater.WebJob.Updater;
using SFA.DAS.Messaging;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.LevyAccountUpdater.UnitTests
{
    public class WhenIUpdateAccounts
    {
        private Mock<IEmployerAccountRepository> _employerAccountRepository;
        private Mock<IMessagePublisher> _messagePublisher;
        private AccountUpdater _updater;
        private List<Account> _accounts;
        private Mock<ILog> _logger;
        private Mock<IEmployerSchemesRepository> _employerSchemesRepository;

        [SetUp]
        public void Init()
        {
            _employerAccountRepository = new Mock<IEmployerAccountRepository>();
            _employerSchemesRepository = new Mock<IEmployerSchemesRepository>();
            _messagePublisher = new Mock<IMessagePublisher>();
            _logger = new Mock<ILog>();

            _accounts = new List<Account>
            {
                new Account { Id = 1 },
                new Account { Id = 2 }
            };

            _employerAccountRepository.Setup(x => x.GetAllAccounts()).ReturnsAsync(_accounts);

            _employerSchemesRepository.Setup(x => x.GetSchemesByEmployerId(It.IsAny<long>())).ReturnsAsync(new PayeSchemes());
            _employerSchemesRepository.Setup(x => x.GetSchemesByEmployerId(1)).ReturnsAsync(new PayeSchemes
            {
                SchemesList = new List<PayeScheme>
                {
                    new PayeScheme
                    {
                        AccountId = 1,
                        Ref = "123ABC"
                    }
                }
            });

            _updater = new AccountUpdater(_employerAccountRepository.Object, _messagePublisher.Object, _logger.Object, _employerSchemesRepository.Object);
        }

        [Test]
        public async Task ThenAnUpdateAccountMessageShouldBeAddedToTheProcessQueueForEachAccountPayeScheme()
        {
            //Act
            await _updater.RunUpdate();

            //Assert
            _employerAccountRepository.Verify(x => x.GetAllAccounts(), Times.Once);
            _messagePublisher.Verify(x => x.PublishAsync(It.IsAny<EmployerRefreshLevyQueueMessage>()), Times.Exactly(1));
            _messagePublisher.Verify(x => x.PublishAsync(It.Is<EmployerRefreshLevyQueueMessage>(m => m.AccountId.Equals(1) && m.PayeRef.Equals("123ABC"))), Times.Once);
            _messagePublisher.Verify(x => x.PublishAsync(It.Is<EmployerRefreshLevyQueueMessage>(m => m.AccountId.Equals(2))), Times.Never);
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
            _logger.Setup(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()));

            //Act
            Assert.ThrowsAsync<Exception>(async () => await _updater.RunUpdate());

            //Assert
            _logger.Verify(x => x.Error(exception, It.IsAny<string>()), Times.Once);
        }
    }
}
