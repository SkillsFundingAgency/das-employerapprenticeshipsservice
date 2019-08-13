using System.Threading.Tasks;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.CreateAccountPaye;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Paye;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.CommandHandlers
{
    public class GivenACreateAccountPayeCommand
    {
        private CreateAccountPayeCommandHandler _handler;
        private Mock<IPayeRepository> _payeRepository;
        private Mock<ILog> _logger;
        
        [SetUp]
        public void Arrange()
        {
            _payeRepository = new Mock<IPayeRepository>();

            _logger = new Mock<ILog>();

            _handler = new CreateAccountPayeCommandHandler(_payeRepository.Object, _logger.Object);
        }

        [Test]
        public async Task ThenThePayeSchemeIsCreated()
        {
            var accountId = 123443;
            var name = "Scheme Name";
            var empRef = "ABC/123246";
            var aorn = "AORN123";

            await _handler.Handle(new CreateAccountPayeCommand(accountId, empRef, name, aorn), Mock.Of<IMessageHandlerContext>());

            _payeRepository.Verify(x => x.CreatePayeScheme(It.Is<Paye>(y => y.Aorn == aorn && y.AccountId == accountId && y.Ref == empRef && y.RefName == name)));
        }
    }
}
