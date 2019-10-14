using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.RemoveAccountPaye;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.RemoveAccountPayeTests
{
    public class WhenIHandleTheCommand
    {
        private RemoveAccountPayeCommandHandler _handler;
        private Mock<IPayeRepository> _payeRepository;
        private Mock<ILog> _logger;
        
        [SetUp]
        public void Arrange()
        {
            _payeRepository = new Mock<IPayeRepository>();

            _logger = new Mock<ILog>();

            _handler = new RemoveAccountPayeCommandHandler(_payeRepository.Object, _logger.Object);
        }

        [Test]
        public async Task ThenThePayeSchemeIsRemoved()
        {
            var accountId = 123443;
            var payeRef = "ABC/12343534";

            await _handler.Handle(new RemoveAccountPayeCommand(accountId, payeRef));

            _payeRepository.Verify(x => x.RemovePayeScheme(accountId, payeRef));
        }
    }
}
