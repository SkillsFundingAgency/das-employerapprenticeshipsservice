using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.RenameAccount;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.RenameAccountTests
{
    public class WhenIHandleTheCommand
    {
        private RenameAccountCommandHandler _handler;
        private Mock<IAccountRepository> _accountRepository;
        private Mock<ILog> _logger;
        
        [SetUp]
        public void Arrange()
        {
            _accountRepository = new Mock<IAccountRepository>();

            _logger = new Mock<ILog>();

            _handler = new RenameAccountCommandHandler(_accountRepository.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheAccountIsRenamed()
        {
            var accountId = 123443;
            var name = "Account Name";

            await _handler.Handle(new RenameAccountCommand(accountId, name));

            _accountRepository.Verify(x => x.RenameAccount(accountId, name));
        }
    }
}
