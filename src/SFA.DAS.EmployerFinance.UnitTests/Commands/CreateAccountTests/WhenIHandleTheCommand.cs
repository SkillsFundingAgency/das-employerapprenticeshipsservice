using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.CreateAccount;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.CreateAccountTests
{
    public class WhenIHandleTheCommand
    {
        private CreateAccountCommandHandler _handler;
        private Mock<IAccountRepository> _accountRepository;
        private Mock<ILog> _logger;
        
        [SetUp]
        public void Arrange()
        {
            _accountRepository = new Mock<IAccountRepository>();

            _logger = new Mock<ILog>();

            _handler = new CreateAccountCommandHandler(_accountRepository.Object, _logger.Object);
        }

        [Test]
        public async Task ThenTheAccountIsCreated()
        {
            var accountId = 123443;
            var name = "Account Name";

            await _handler.Handle(new CreateAccountCommand(accountId, name));

            _accountRepository.Verify(x => x.CreateAccount(accountId, name));
        }
    }
}
