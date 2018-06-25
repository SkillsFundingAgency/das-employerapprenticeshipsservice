﻿using FluentAssertions;
using MediatR;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.RenameEmployerAccount;
using SFA.DAS.EAS.Application.Factories;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.HashingService;
using System;
using System.Linq;
using System.Threading.Tasks;
using IGenericEventFactory = SFA.DAS.EAS.Application.Factories.IGenericEventFactory;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RenameEmployerAccountCommandTests
{
    public class WhenICallRenameAccount
    {
        private Mock<IEmployerAccountRepository> _repository;
        private Mock<IValidator<RenameEmployerAccountCommand>> _validator;
        private Mock<IHashingService> _hashingService;
        private Mock<IMediator> _mediator;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IGenericEventFactory> _genericEventFactory;
        private const long AccountId = 12343322;
        private const string AccountName = "TestAccount";
        private const string HashedAccountId = "123ADF23";
        private RenameEmployerAccountCommandHandler _commandHandler;
        private RenameEmployerAccountCommand _command;
        private MembershipView _owner;
        private Mock<IAccountEventFactory> _accountEventFactory;
        private TestableEndpointInstance _endpoint;

        [SetUp]
        public void Arrange()
        {
            _command = new RenameEmployerAccountCommand
            {
                HashedAccountId = HashedAccountId,
                NewName = "Renamed account"
            };

            _owner = new MembershipView
            {
                AccountId = 1234,
                UserId = 9876,
                UserRef = Guid.NewGuid().ToString(),
                Email = "test@test.com",
                FirstName = "Bob",
                LastName = "Green"
            };

            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(_owner);

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(It.Is<string>(s => s == HashedAccountId)))
                .Returns(AccountId);

            _repository = new Mock<IEmployerAccountRepository>();
            _repository.Setup(x => x.GetAccountById(It.IsAny<long>()))
                .ReturnsAsync(new Domain.Models.Account.Account
                {
                    Id = AccountId,
                    HashedId = HashedAccountId,
                    Name = AccountName
                });

            _validator = new Mock<IValidator<RenameEmployerAccountCommand>>();

            _validator.Setup(x => x.ValidateAsync(It.IsAny<RenameEmployerAccountCommand>()))
                .ReturnsAsync(new ValidationResult());

            _mediator = new Mock<IMediator>();
            _genericEventFactory = new Mock<IGenericEventFactory>();
            _accountEventFactory = new Mock<IAccountEventFactory>();
            _endpoint = new TestableEndpointInstance();

            _commandHandler = new RenameEmployerAccountCommandHandler(
                _endpoint,
                _repository.Object,
                _membershipRepository.Object,
                _validator.Object,
                _hashingService.Object,
                _mediator.Object,
                _genericEventFactory.Object,
                _accountEventFactory.Object);
        }

        [Test]
        public async Task ThenTheAccountIsRenamedToTheNewName()
        {
            //Act
            await _commandHandler.Handle(_command);

            //Assert
            _repository.Verify(x => x.RenameAccount(It.Is<long>(l => l == AccountId), It.Is<string>(s => s == _command.NewName)));
        }

        [Test]
        public async Task ThenTheAuditCommandIsCalledWhenTheCommandIsValid()
        {
            //Act
            await _commandHandler.Handle(_command);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(AccountId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Name") && y.NewValue.Equals(_command.NewName)) != null)));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"User {_owner.Email} has renamed account {AccountId} to {_command.NewName}"))));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(AccountId.ToString()) && y.Type.Equals("Account")) != null)));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(AccountId.ToString()) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("Account"))));
        }

        [Test]
        public async Task ThenAnAccountRenamedEventIsPublished()
        {
            //Act
            await _commandHandler.Handle(_command);

            //Assert
            var publishedMessages = _endpoint.PublishedMessages;
            publishedMessages.Length.Should().Be(1);
            publishedMessages.First().Message.Should().BeAssignableTo<IChangedAccountNameEvent>();
        }

        [Test]
        public void ThenAnAccountRenamedEventIsNotPublishedIfRenamingFails()
        {
            //Arrange
            _repository.Setup(x => x.RenameAccount(It.IsAny<long>(), It.IsAny<string>())).Throws<Exception>();

            //Act
            Assert.ThrowsAsync<Exception>(() => _commandHandler.Handle(_command));

            //Assert
            _endpoint.SentMessages.Should().BeEmpty();
        }
    }
}
