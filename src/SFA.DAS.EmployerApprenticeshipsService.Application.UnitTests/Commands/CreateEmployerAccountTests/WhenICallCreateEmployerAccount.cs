using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
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
        private Mock<IMediator> _mediator;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IAccountRepository>();
            _messagePublisher = new Mock<IMessagePublisher>();
            _mediator = new Mock<IMediator>();
            _handler = new CreateAccountCommandHandler(_repository.Object, _messagePublisher.Object, _mediator.Object);
        }

        [Test]
        public async Task WillCallRepositoryToCreateNewAccount()
        {
            const int accountId = 23;

            var cmd = new CreateAccountCommand
            {
                UserId = Guid.NewGuid().ToString(),
                CompanyNumber = "QWERTY",
                CompanyName = "Qwerty Corp",
                EmployerRef = "120/QWERTY"
            };

            _repository.Setup(x => x.CreateAccount(cmd.UserId, cmd.CompanyNumber, cmd.CompanyName, cmd.EmployerRef)).ReturnsAsync(accountId);

            await _handler.Handle(cmd);

            _repository.Verify(x => x.CreateAccount(It.Is<string>(c => c.Equals(cmd.UserId)), It.Is<string>(c => c.Equals(cmd.CompanyNumber)), It.Is<string>(c => c.Equals(cmd.CompanyName)), It.Is<string>(c => c.Equals(cmd.EmployerRef))), Times.Once);

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