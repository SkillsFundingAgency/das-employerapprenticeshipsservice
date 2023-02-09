using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Factories;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.Encoding;
using SFA.DAS.NServiceBus.Testing.Services;


namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.RenameEmployerAccountCommandTests
{
    public class WhenICallRenameAccount
    {
        private Mock<IEmployerAccountRepository> _repository;
        private Mock<IValidator<RenameEmployerAccountCommand>> _validator;
        private Mock<IEncodingService> _encodingService;
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
        private TestableEventPublisher _eventPublisher;

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
                UserRef = Guid.NewGuid(),
                Email = "test@test.com",
                FirstName = "Bob",
                LastName = "Green"
            };

            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(_owner);

            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Decode(HashedAccountId, EncodingType.AccountId)).Returns(AccountId);

            _repository = new Mock<IEmployerAccountRepository>();
            _repository.Setup(x => x.GetAccountById(It.IsAny<long>()))
                .ReturnsAsync(new Account
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
            _eventPublisher = new TestableEventPublisher();

            _commandHandler = new RenameEmployerAccountCommandHandler(
                _eventPublisher,
                _repository.Object,
                _membershipRepository.Object,
                _validator.Object,
                _encodingService.Object,
                _mediator.Object,
                _genericEventFactory.Object,
                _accountEventFactory.Object);
        }

        [Test]
        public async Task ThenTheAccountIsRenamedToTheNewName()
        {
            //Act
            await _commandHandler.Handle(_command, CancellationToken.None);

            //Assert
            _repository.Verify(x => x.RenameAccount(It.Is<long>(l => l == AccountId), It.Is<string>(s => s == _command.NewName)));
        }

        [Test]
        public async Task ThenTheAuditCommandIsCalledWhenTheCommandIsValid()
        {
            //Act
            await _commandHandler.Handle(_command, CancellationToken.None);

            //Assert
            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(AccountId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Name") && y.NewValue.Equals(_command.NewName)) != null), It.IsAny<CancellationToken>()));

            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"User {_owner.Email} has renamed account {AccountId} to {_command.NewName}")), It.IsAny<CancellationToken>()));

            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(AccountId.ToString()) && y.Type.Equals("Account")) != null), It.IsAny<CancellationToken>()));

            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(AccountId.ToString()) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("Account")), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenAnAccountRenamedEventIsPublished()
        {
            //Act
            await _commandHandler.Handle(_command, CancellationToken.None);

            //Assert

            _eventPublisher.Events.Should().HaveCount(1);
            _eventPublisher.Events.First().Should().BeOfType<ChangedAccountNameEvent>();
        }

        [Test]
        public void ThenAnAccountRenamedEventIsNotPublishedIfRenamingFails()
        {
            //Arrange
            _repository.Setup(x => x.RenameAccount(It.IsAny<long>(), It.IsAny<string>())).Throws<Exception>();

            //Act
            Assert.ThrowsAsync<Exception>(() => _commandHandler.Handle(_command, CancellationToken.None));

            //Assert
            _eventPublisher.Events.Should().BeEmpty();
        }
    }
}
