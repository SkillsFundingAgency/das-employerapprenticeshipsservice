using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.Commands.CreateEmployerAccountTests
{
    [TestFixture]
    public class WhenICallCreateEmployerAccount
    {
        private Mock<IAccountRepository> _repository;
        private CreateAccountCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IAccountRepository>();
            _handler = new CreateAccountCommandHandler(_repository.Object);
        }

        [Test]
        public async Task WillCallRepositoryToCreateNewAccount()
        {
            var cmd = new CreateAccountCommand
            {
                UserId = Guid.NewGuid().ToString(),
                CompanyNumber = "QWERTY",
                CompanyName = "Qwerty Corp",
                EmployerRef = "120/QWERTY"
            };

            await _handler.Handle(cmd);

            _repository.Verify(x => x.CreateAccount(It.Is<string>(c => c.Equals(cmd.UserId)), It.Is<string>(c => c.Equals(cmd.CompanyNumber)), It.Is<string>(c => c.Equals(cmd.CompanyName)), It.Is<string>(c => c.Equals(cmd.EmployerRef))), Times.Once);
        }
    }
}