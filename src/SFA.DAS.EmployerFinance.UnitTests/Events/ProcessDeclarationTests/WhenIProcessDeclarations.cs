using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Events.ProcessDeclaration;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerFinance.UnitTests.Events.ProcessDeclarationTests
{
    [ExcludeFromCodeCoverage]
    public class WhenIProcessDeclarations
    {
        private ProcessDeclarationsEventHandler _processDeclarationsEvent;
        private Mock<IDasLevyRepository> _dasLevyRepository;
        private Mock<ILog> _logger;
        private Mock<IEventPublisher> _eventPublisher;

        [SetUp]
        public void Arrange()
        {
            _eventPublisher = new Mock<IEventPublisher>();
            _dasLevyRepository = new Mock<IDasLevyRepository>();
            _logger = new Mock<ILog>();

            _processDeclarationsEvent = new ProcessDeclarationsEventHandler(
                _dasLevyRepository.Object,
                _logger.Object,
                _eventPublisher.Object);
        }

        [Test]
        public async Task ThenTheProcessDeclarationsRepositoryCallIsMade()
        {
            //Arrange
            const long accountId = 2L;
            const string empRef = "123/ABC456";

            //Act
            await _processDeclarationsEvent.Handle(new ProcessDeclarationsEvent
            {
                AccountId = accountId,
                EmpRef = empRef
            });

            //Assert
            _dasLevyRepository.Verify(x => x.ProcessDeclarations(accountId, empRef), Times.Once);
        }

        [Test]
        public async Task ThenTheLoggerIsCalledWithInfoLevel()
        {
            //Act
            await _processDeclarationsEvent.Handle(new ProcessDeclarationsEvent());

            //Assert
            _logger.Verify(x=>x.Info("Process Declarations Called"));
        }

        [Test]
        public async Task ThenLevyAddedToAccountEventIsPublishedForGreaterThanZeroLevyAmount()
        {
            long accountId = 900034;
            decimal levyAmount = 10.0m;

            await
                SetupRepositoryAndHandleMessage(
                    accountId,
                    levyAmount);

                _eventPublisher
                .Verify(
                    m =>
                        m.Publish(
                            It.Is<LevyAddedToAccount>(
                                arg => arg.AccountId == accountId
                                       && arg.Amount == levyAmount)),
                    Times.Once);
        }

        [Test]
        public async Task ThenLevyAddedToAccountEventIsNotPublishedForZeroLevyAmount()
        {
            long accountId = 900034;
            decimal levyAmount = decimal.Zero;

            await
            SetupRepositoryAndHandleMessage(
                accountId,
                levyAmount);

            _eventPublisher
                .Verify(
                    m =>
                        m.Publish(
                            It.Is<LevyAddedToAccount>(
                                arg => arg.AccountId == accountId
                                       && arg.Amount == levyAmount)),
                    Times.Never);
        }

        [Test]
        public async Task ThenLevyAddedToAccountEventIsNotPublishedForLessThanZeroLevyAmount()
        {
            long accountId = 900034;
            decimal levyAmount = decimal.MinusOne;

            await
                SetupRepositoryAndHandleMessage(
                    accountId,
                    levyAmount);

            _eventPublisher
                .Verify(
                       m =>
                        m.Publish(
                            It.Is<LevyAddedToAccount>(
                                arg => arg.AccountId == accountId
                                       && arg.Amount == levyAmount)),
                    Times.Never);
        }

        private async Task SetupRepositoryAndHandleMessage(long accountId, decimal levyAmount)
        {
            _dasLevyRepository
                .Setup(
                    m => m.ProcessDeclarations(
                        accountId,
                        It.IsAny<string>()))
                .Returns(
                    Task.FromResult(levyAmount));

            await
            _processDeclarationsEvent
                .Handle(
                    new ProcessDeclarationsEvent
                    {
                        AccountId = accountId
                    });
        }
    }
}
