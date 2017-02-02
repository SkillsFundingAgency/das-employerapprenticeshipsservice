using System.Threading.Tasks;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Events.ProcessPayment;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.UnitTests.Events.ProcessPaymentTests
{
    public class WhenIProcessPaymentData
    {
        private ProcessPaymentEventHandler _eventHandler;
        private Mock<IDasLevyRepository> _dasLevyRepository;
        private Mock<ILogger> _logger;

        [SetUp]
        public void Arrange()
        {
            _dasLevyRepository = new Mock<IDasLevyRepository>();
            _logger = new Mock<ILogger>();

            _eventHandler = new ProcessPaymentEventHandler(_dasLevyRepository.Object,_logger.Object);
        }

        [Test]
        public async Task ThenTheProcessDeclarationsRepositoryCallIsMade()
        {
            //Act
            await _eventHandler.Handle(new ProcessPaymentEvent());

            //Assert
            _dasLevyRepository.Verify(x => x.ProcessPaymentData(), Times.Once);
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
