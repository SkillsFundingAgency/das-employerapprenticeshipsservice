using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Events.ProcessDeclaration;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Events.ProcessDeclarationTests
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
    }
}
