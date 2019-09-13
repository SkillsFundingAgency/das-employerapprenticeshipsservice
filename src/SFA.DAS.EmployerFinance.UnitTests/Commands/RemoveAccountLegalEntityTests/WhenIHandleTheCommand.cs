using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.RemoveAccountLegalEntity;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.RemoveAccountLegalEntityTests
{
    public class WhenIHandleTheCommand
    {
        private RemoveAccountLegalEntityCommandHandler _handler;
        private Mock<IAccountLegalEntityRepository> _accountLegalEntityRepository;

        [SetUp]
        public void Arrange()
        {
            _accountLegalEntityRepository = new Mock<IAccountLegalEntityRepository>();

            _handler = new RemoveAccountLegalEntityCommandHandler(_accountLegalEntityRepository.Object, Mock.Of<ILog>());
        }

        [Test]
        public async Task ThenTheAccountLegalEntityIsRemoved()
        {
            var id = 234985;

            await _handler.Handle(new RemoveAccountLegalEntityCommand(id));

            _accountLegalEntityRepository.Verify(x => x.RemoveAccountLegalEntity(id));
        }
    }
}
