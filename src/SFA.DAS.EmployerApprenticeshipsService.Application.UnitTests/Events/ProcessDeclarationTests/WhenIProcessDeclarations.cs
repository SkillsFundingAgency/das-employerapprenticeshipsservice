using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Events.ProcessDeclaration;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.UnitTests.Events.ProcessDeclarationTests
{
    public class WhenIProcessDeclarations
    {
        private ProcessDeclarationsEventHandler _processDeclarationsEvent;
        private Mock<IDasLevyRepository> _dasLevyRepository;
        private Mock<ILog> _logger;

        [SetUp]
        public void Arrange()
        {
            _dasLevyRepository = new Mock<IDasLevyRepository>();
            _logger = new Mock<ILog>();

            _processDeclarationsEvent = new ProcessDeclarationsEventHandler(_dasLevyRepository.Object, _logger.Object);    
        }

        [Test]
        public async Task ThenTheProcessDeclarationsRepositoryCallIsMade()
        {
            //Act
            await _processDeclarationsEvent.Handle(new ProcessDeclarationsEvent());

            //Assert
            _dasLevyRepository.Verify(x=>x.ProcessDeclarations(), Times.Once);
        }

        [Test]
        public async Task ThenTheLoggerIsCalledWithInfoLevel()
        {
            //Act
            await _processDeclarationsEvent.Handle(new ProcessDeclarationsEvent());

            //Assert
            _logger.Verify(x=>x.Info("Process Declarations Called"));
        }
    }
}
