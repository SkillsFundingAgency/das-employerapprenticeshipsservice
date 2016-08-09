using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.CreateEmployerAccountTests
{
    [TestFixture]
    public class WhenICallCreateEmployerAccount
    {
        private Mock<IAccountRepository> _repository;
        private CreateAccountCommandHandler _handler;
        private Mock<IMessagePublisher> _messagePublisher;
        private Mock<IUserRepository> _userRepository;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IAccountRepository>();
            _userRepository = new Mock<IUserRepository>();
            _messagePublisher = new Mock<IMessagePublisher>();
            _handler = new CreateAccountCommandHandler(_repository.Object, _userRepository.Object, _messagePublisher.Object);
        }

        [Test]
        public async Task WillCallRepositoryToCreateNewAccount()
        {
            const int accountId = 23;

            var user = new User()
            {
                Id = 33
            };

            var cmd = new CreateAccountCommand
            {
                ExternalUserId = Guid.NewGuid().ToString(),
                CompanyNumber = "QWERTY",
                CompanyName = "Qwerty Corp",
                CompanyRegisteredAddress = "Innovation Centre, Coventry, CV1 2TT",
                CompanyDateOfIncorporation = DateTime.Today.AddDays(-1000),
                EmployerRef = "120/QWERTY"
            };

            _userRepository.Setup(x => x.GetById(cmd.ExternalUserId)).ReturnsAsync(user);
            _repository.Setup(x => x.CreateAccount(user.Id, cmd.CompanyNumber, cmd.CompanyName, cmd.CompanyRegisteredAddress, cmd.CompanyDateOfIncorporation, cmd.EmployerRef)).ReturnsAsync(accountId);

            await _handler.Handle(cmd);

            _repository.Verify(x => x.CreateAccount(It.Is<long>(c => c.Equals(user.Id)), It.Is<string>(c => c.Equals(cmd.CompanyNumber)), It.Is<string>(c => c.Equals(cmd.CompanyName)), It.Is<string>(c => c.Equals(cmd.CompanyRegisteredAddress)), It.Is<DateTime>(c => c.Equals(cmd.CompanyDateOfIncorporation)), It.Is<string>(c => c.Equals(cmd.EmployerRef))), Times.Once);

            _messagePublisher.Verify(x => x.PublishAsync(It.Is<EmployerRefreshLevyQueueMessage>(c => c.AccountId == accountId)), Times.Once());
        }

        [Test]
        public void InvalidRequest()
        {
            var command = new CreateAccountCommand();

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(4));
        }
    }
}