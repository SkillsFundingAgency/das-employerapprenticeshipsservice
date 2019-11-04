using System.Threading.Tasks;
using Moq;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.MessageHandlers.CommandHandlers;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.EmployerFinance.Models.Paye;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.MessageHandlers.UnitTests.CommandHandlers
{
    public class GivenAPayeSchemeIsAddedToAnAccount
    {
        private CreateAccountPayeCommandHandler _handler;
        private Mock<IPayeRepository> _payeRepository;
        private Mock<ILog> _logger;
        private Mock<IMessageHandlerContext> _context;

        [SetUp]
        public void Arrange()
        {
            _payeRepository = new Mock<IPayeRepository>();
            _context = new Mock<IMessageHandlerContext>();
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

            await _handler.Handle(new CreateAccountPayeCommand(accountId, empRef, name, aorn), _context.Object);

            _payeRepository.Verify(x => x.CreatePayeScheme(It.Is<Paye>(y => y.Aorn == aorn && y.AccountId == accountId && y.EmpRef == empRef && y.Name == name)));
        }

        [Test]
        public async Task IfTheSchemeWasNotAddedViaAornThenLevyIsAddedToTheAccount()
        {
            var accountId = 123443;
            var name = "Scheme Name";
            var empRef = "ABC/123246";
            var aorn = string.Empty;

            await _handler.Handle(new CreateAccountPayeCommand(accountId, empRef, name, aorn), _context.Object);

            _context.Verify(x => x.Send(It.Is<ImportAccountLevyDeclarationsCommand>(y => y.AccountId == accountId && y.PayeRef == empRef), It.IsAny<SendOptions>()));
        }

        [Test]
        public async Task IfTheSchemeWasAddedViaAornThenLevyIsNotAddedToTheAccount()
        {
            var accountId = 123443;
            var name = "Scheme Name";
            var empRef = "ABC/123246";
            var aorn = "AORN";

            await _handler.Handle(new CreateAccountPayeCommand(accountId, empRef, name, aorn), _context.Object);

            _context.Verify(x => x.Send(It.IsAny<ImportAccountLevyDeclarationsCommand>(), It.IsAny<SendOptions>()), Times.Never());
        }
    }
}
