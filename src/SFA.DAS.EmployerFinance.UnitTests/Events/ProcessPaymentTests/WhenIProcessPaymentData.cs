using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Events.ProcessPayment;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Events.ProcessPaymentTests
{
    public class WhenIProcessPaymentData
    {
        private ProcessPaymentEventHandler _eventHandler;
        private Mock<IDasLevyRepository> _dasLevyRepository;
        private Mock<ILog> _logger;

        [SetUp]
        public void Arrange()
        {
            _dasLevyRepository = new Mock<IDasLevyRepository>();
            _logger = new Mock<ILog>();

            _eventHandler = new ProcessPaymentEventHandler(_dasLevyRepository.Object,_logger.Object);
        }

        [Test]
        public async Task ThenTheProcessDeclarationsRepositoryCallIsMade()
        {
            //Arrange
            const int accountId = 10;

            //Act
            await _eventHandler.Handle(new ProcessPaymentEvent{AccountId = accountId });

            //Assert
            _dasLevyRepository.Verify(x => x.ProcessPaymentData(accountId), Times.Once);
        }

        [Test]
        public async Task ThenTheLoggerIsCalledWithInfoLevel()
        {
            //Act
            await _eventHandler.Handle(new ProcessPaymentEvent());

            //Assert
            _logger.Verify(x => x.Info("Process Payments Called"));
        }
    }
}
