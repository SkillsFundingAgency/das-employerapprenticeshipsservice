using FluentAssertions;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.AcceptInvitation;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Messages.Events;
using SFA.DAS.TimeProvider;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.AcceptInvitationTests
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
        private TestableEndpointInstance _endpoint;
        private Mock<IValidator<AcceptInvitationCommand>> _validator;

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
                RoleId = Role.Owner,
                Name = "Bob Green"
            };

            _invitationRepository = new Mock<IInvitationRepository>();
            _membershipRepository = new Mock<IMembershipRepository>();
            _userAccountRepository = new Mock<IUserAccountRepository>();
            _auditService = new Mock<IAuditService>();
            _endpoint = new TestableEndpointInstance();
            _validator = new Mock<IValidator<AcceptInvitationCommand>>();

            _validator.Setup(x => x.Validate(It.IsAny<AcceptInvitationCommand>())).Returns(new ValidationResult());

            _membershipRepository.Setup(x => x.GetCaller(It.IsAny<long>(), It.IsAny<string>())).ReturnsAsync(null);

            _invitationRepository.Setup(x => x.Get(It.IsAny<long>())).ReturnsAsync(_invitation);
            _userAccountRepository.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(new User { UserRef = Guid.NewGuid().ToString() });

            _handler = new AcceptInvitationCommandHandler(
                _invitationRepository.Object,
                _membershipRepository.Object,
                _userAccountRepository.Object,
                _auditService.Object,
                _endpoint,
                _validator.Object,
                Mock.Of<ILog>());
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
            await _handler.Handle(new AcceptInvitationCommand());

            //Assert
            _invitationRepository.Verify(x => x.Accept(_invitation.Email, _invitation.AccountId, (short)_invitation.RoleId), Times.Once);
        }

        [Test]
        public async Task ThenShouldAuditWheninviteHasBeenAccepted()
        {
            //Act
            await _handler.Handle(new AcceptInvitationCommand());

            //Assert
            _auditService.Verify(x => x.SendAuditMessage(It.IsAny<EasAuditMessage>()), Times.Once);
        }

        [Test]
        public void ThenIfUserIsNotFound()
        {
            //Assign
            _userAccountRepository.Setup(x => x.Get(_invitation.Email)).ReturnsAsync(null);

            //Act + Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(new AcceptInvitationCommand()));
        }

        [Test]
        public void ThenIfUserIsAlreadyATeamMember()
        {
            //Assign
            _membershipRepository.Setup(a => a.GetCaller(It.IsAny<long>(), It.IsAny<string>()))
                                 .Returns(Task.FromResult(new MembershipView { FirstName = "Bob", LastName = "Green" }));

            //Act + Assert
            Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(new AcceptInvitationCommand()));
        }

        [Test]
        public void ThenIfTheInvitationDoesNotExist()
        {
            //Assign
            _invitationRepository.Setup(x => x.Get(It.IsAny<long>())).ReturnsAsync(null);

            //Act + Assert


            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(new AcceptInvitationCommand()));
        }

        [Test]
        public void ThenIfInvitationStatusIsNotPending()
        {
            //Assign
            _invitation.Status = InvitationStatus.Accepted;

            //Act + Assert


            Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(new AcceptInvitationCommand()));
        }

        [Test]
        public void ThenIfInvitationHasExpired()
        {
            //Assign
            _invitation.ExpiryDate = DateTimeProvider.Current.UtcNow.AddDays(-2);


            //Act + Assert


            Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(new AcceptInvitationCommand()));
        }

        [Test]
        public void ThenIfTheRequestIsInvalid()
        {
            //Assign
            _invitationRepository.Setup(x => x.Get(It.IsAny<long>())).ReturnsAsync(null);

            //Act + Assert
            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(new AcceptInvitationCommand()));
        }

        [Test]
        public async Task ThenTheUserFullNameShouldBeUsed()
        {
            //Assign
            var user = new User { FirstName = "Bill", LastName = "Green", UserRef = Guid.NewGuid().ToString() };

            _userAccountRepository.Setup(x => x.Get(_invitation.Email))
                                 .ReturnsAsync(user);

            //Act
            await _handler.Handle(new AcceptInvitationCommand());

            //Assert
            _endpoint.PublishedMessages.Should().HaveCount(1);

            var message = _endpoint.PublishedMessages.Select(x => x.Message)
                                                     .OfType<UserJoinedEvent>()
                                                     .Single();

            message.UserName.Should().Be(user.FullName);
        }
    }
}