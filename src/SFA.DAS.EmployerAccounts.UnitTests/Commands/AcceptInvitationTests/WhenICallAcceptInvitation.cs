using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AcceptInvitation;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.Encoding;
using SFA.DAS.NServiceBus.Testing.Services;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.AcceptInvitationTests
{
    [TestFixture]
    public class WhenICallAcceptInvitation
    {
        private AcceptInvitationCommandHandler _handler;
        private Invitation _invitation;
        private Mock<IInvitationRepository> _invitationRepository;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IUserAccountRepository> _userAccountRepository;
        private Mock<IAuditService> _auditService;
        private TestableEventPublisher _eventPublisher;
        private Mock<IValidator<AcceptInvitationCommand>> _validator;
        private Mock<IEncodingService> _encodingService;

        [SetUp]
        public void Setup()
        {
            _invitation = new Invitation
            {
                Id = 1,
                AccountId = 101,
                Email = "test.user@test.local",
                Status = InvitationStatus.Pending,
                ExpiryDate = DateTimeProvider.Current.UtcNow.AddDays(2),
                Role = Role.Owner,
                Name = "Bob Green"
            };

            _invitationRepository = new Mock<IInvitationRepository>();
            _membershipRepository = new Mock<IMembershipRepository>();
            _userAccountRepository = new Mock<IUserAccountRepository>();
            _auditService = new Mock<IAuditService>();
            _eventPublisher = new TestableEventPublisher();
            _validator = new Mock<IValidator<AcceptInvitationCommand>>();
            _encodingService = new Mock<IEncodingService>();

            _validator.Setup(x => x.Validate(It.IsAny<AcceptInvitationCommand>())).Returns(new ValidationResult());

            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(() => null);

            _invitationRepository.Setup(x => x.Get(It.IsAny<long>())).ReturnsAsync(_invitation);
            _userAccountRepository.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(new User { UserRef = Guid.NewGuid().ToString() });

            _handler = new AcceptInvitationCommandHandler(
                _invitationRepository.Object,
                _membershipRepository.Object,
                _userAccountRepository.Object,
                _auditService.Object,
                _eventPublisher,
                _validator.Object,
                _encodingService.Object,
                Mock.Of<ILogger<AcceptInvitationCommandHandler>>());
        }

        [TearDown]
        public void Teardown()
        {
            DateTimeProvider.ResetToDefault();
        }

        [Test]
        public async Task ThenTheInviatationWillBeAccepted()
        {
            //Act
            await _handler.Handle(new AcceptInvitationCommand(), CancellationToken.None);

            //Assert
            _invitationRepository.Verify(x => x.Accept(_invitation.Email, _invitation.AccountId, _invitation.Role), Times.Once);
        }

        [Test]
        public async Task ThenShouldAuditWheninviteHasBeenAccepted()
        {
            //Act
            await _handler.Handle(new AcceptInvitationCommand(), CancellationToken.None);

            //Assert
            _auditService.Verify(x => x.SendAuditMessage(It.IsAny<AuditMessage>()), Times.Once);
        }

        [Test]
        public void ThenIfUserIsNotFound()
        {
            //Assign
            _userAccountRepository.Setup(x => x.Get(_invitation.Email)).ReturnsAsync(() => null);

            //Act + Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(new AcceptInvitationCommand(), CancellationToken.None));
        }

        [Test]
        public void ThenIfUserIsAlreadyATeamMember()
        {
            //Assign
            _membershipRepository.Setup(a => a.GetCaller(It.IsAny<long>(), It.IsAny<string>()))
                                 .Returns(Task.FromResult(new MembershipView { FirstName = "Bob", LastName = "Green" }));

            //Act + Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(new AcceptInvitationCommand(), CancellationToken.None));
        }

        [Test]
        public void ThenIfTheInvitationDoesNotExist()
        {
            //Assign
            _invitationRepository.Setup(x => x.Get(It.IsAny<long>())).ReturnsAsync(() => null);

            //Act + Assert


            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(new AcceptInvitationCommand(), CancellationToken.None));
        }

        [Test]
        public void ThenIfInvitationStatusIsNotPending()
        {
            //Assign
            _invitation.Status = InvitationStatus.Accepted;

            //Act + Assert


            Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(new AcceptInvitationCommand(), CancellationToken.None));
        }

        [Test]
        public void ThenIfInvitationHasExpired()
        {
            //Assign
            _invitation.ExpiryDate = DateTimeProvider.Current.UtcNow.AddDays(-2);


            //Act + Assert


            Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(new AcceptInvitationCommand(), CancellationToken.None));
        }

        [Test]
        public void ThenIfTheRequestIsInvalid()
        {
            //Assign
            _invitationRepository.Setup(x => x.Get(It.IsAny<long>())).ReturnsAsync(() => null);

            //Act + Assert
            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(new AcceptInvitationCommand(), CancellationToken.None));
        }

        [Test]
        public async Task ThenTheUserJoinedEventShouldContainUsersProperties()
        {
            //Assign
            var user = new User { FirstName = "Bill", LastName = "Green", UserRef = Guid.NewGuid().ToString() };

            _userAccountRepository.Setup(x => x.Get(_invitation.Email))
                                 .ReturnsAsync(user);

            var expectedHashedId = "HASHED";
            _encodingService.Setup(x => x.Encode(_invitation.AccountId, EncodingType.AccountId)).Returns(expectedHashedId);

            //Act
            await _handler.Handle(new AcceptInvitationCommand(), CancellationToken.None);

            //Assert
            _eventPublisher.Events.Should().HaveCount(1);

            var message = _eventPublisher.Events.OfType<UserJoinedEvent>().Single();

            message.UserRef.Should().Be(Guid.Parse(user.UserRef));
            message.AccountId.Should().Be(_invitation.AccountId);
            message.HashedAccountId.Should().Be(expectedHashedId);
            message.UserName.Should().Be(user.FullName);
            message.Role.Should().Be((UserRole)_invitation.Role);
        }
    }
}