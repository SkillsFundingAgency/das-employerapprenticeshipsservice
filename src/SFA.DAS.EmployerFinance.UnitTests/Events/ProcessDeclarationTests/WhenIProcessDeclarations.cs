using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Events.ProcessDeclaration;
using SFA.DAS.EmployerFinance.Messages.Events;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.EmployerFinance.UnitTests.Events.ProcessDeclarationTests
{
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

        [TestCase(100023,0.0,false)]
        [TestCase(900034,10.0,true)]
        public async Task ThenLevyAddedToAccountEventIsPublishedForGreaterThanZeroLevyAmount(
            long accountId,
            decimal levyAmount,
            bool eventShouldBePublished)
        {
            _dasLevyRepository
                .Setup(
                    m => m.ProcessDeclarations(
                        accountId,
                        It.IsAny<string>()))
                .Returns(
                    Task.FromResult(levyAmount));

            _processDeclarationsEvent
                .Handle(
                    new ProcessDeclarationsEvent
                    {
                        AccountId = accountId
                    });

            var numberOfExpectedCalls = eventShouldBePublished ? Times.Once() : Times.Never();

            _eventPublisher
                .Verify(
                    m =>
                        m.Publish(
                            It.Is<LevyAddedToAccount>(
                                arg => arg.AccountId == accountId
                                       && arg.Amount == levyAmount)),
                    numberOfExpectedCalls);
        }
    }
}
