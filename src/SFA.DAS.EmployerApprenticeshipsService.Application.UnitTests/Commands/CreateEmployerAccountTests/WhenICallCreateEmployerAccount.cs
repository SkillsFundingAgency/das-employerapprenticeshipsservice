﻿using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAccount;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.Commands.CreateEmployerAccountTests
{
    [TestFixture]
    public class WhenICallCreateEmployerAccount
    {
        private Mock<IAccountRepository> _repository;
        private CreateAccountCommandHandler _handler;
        private Mock<IMessagePublisher> _messagePublisher;

        [SetUp]
        public void Setup()
        {
            _repository = new Mock<IAccountRepository>();
            _messagePublisher = new Mock<IMessagePublisher>();
            _handler = new CreateAccountCommandHandler(_repository.Object, _messagePublisher.Object);
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
    }
}