using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ResendInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.ResendInvitationTests
{
    [TestFixture]
    public class WhenIResendAnInvitation
    {
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IInvitationRepository> _invitationRepository;
        private ResendInvitationCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _invitationRepository = new Mock<IInvitationRepository>();
            _handler = new ResendInvitationCommandHandler(_invitationRepository.Object, _membershipRepository.Object);
        }

        [TearDown]
        public void Teardown()
        {
            DateTimeProvider.ResetToDefault();
        }

        [Test]
        public void InvalidCommandThrowsException()
        {
            var command = new ResendInvitationCommand();

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(3));

            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Id"), Is.Not.Null);
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "AccountId"), Is.Not.Null);
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "ExternalUserId"), Is.Not.Null);
        }

        [Test]
        public void CallerIsNotAnAccountOwner()
        {
            var command = new ResendInvitationCommand
            {
                Id = 1,
                AccountId = 2,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var owner = new MembershipView
            {
                AccountId = command.AccountId,
                UserId = 2,
                RoleId = (int)Role.Viewer
            };

            _membershipRepository.Setup(x => x.GetCaller(command.AccountId, command.ExternalUserId)).ReturnsAsync(owner);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));

            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);
        }

        [Test]
        public void InvitationDoesNotExist()
        {
            var command = new ResendInvitationCommand
            {
                Id = 1,
                AccountId = 2,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var owner = new MembershipView
            {
                AccountId = command.AccountId,
                UserId = 2,
                RoleId = (int)Role.Owner
            };

            _membershipRepository.Setup(x => x.GetCaller(command.AccountId, command.ExternalUserId)).ReturnsAsync(owner);
            _invitationRepository.Setup(x => x.Get(command.Id)).ReturnsAsync(null);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));

            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Invitation"), Is.Not.Null);
        }

        [Test]
        public void AcceptedInvitationsCannotBeResent()
        {
            var command = new ResendInvitationCommand
            {
                Id = 1,
                AccountId = 2,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var owner = new MembershipView
            {
                AccountId = command.AccountId,
                UserId = 2,
                RoleId = (int)Role.Owner
            };

            var invitation = new Invitation
            {
                Id = command.Id,
                AccountId = command.AccountId,
                Status = InvitationStatus.Accepted
            };

            _membershipRepository.Setup(x => x.GetCaller(command.AccountId, command.ExternalUserId)).ReturnsAsync(owner);
            _invitationRepository.Setup(x => x.Get(command.Id)).ReturnsAsync(invitation);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));

            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Invitation"), Is.Not.Null);
        }

        [Test]
        public async Task ShouldResendInvitation()
        {
            DateTimeProvider.Current = new FakeTimeProvider(DateTime.Now);

            var command = new ResendInvitationCommand
            {
                Id = 1,
                AccountId = 2,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var owner = new MembershipView
            {
                AccountId = command.AccountId,
                UserId = 2,
                RoleId = (int)Role.Owner
            };

            var invitation = new Invitation
            {
                Id = command.Id,
                AccountId = command.AccountId,
                Status = InvitationStatus.Deleted,
                ExpiryDate = DateTimeProvider.Current.UtcNow.AddDays(-1)
            };

            _membershipRepository.Setup(x => x.GetCaller(command.AccountId, command.ExternalUserId)).ReturnsAsync(owner);
            _invitationRepository.Setup(x => x.Get(command.Id)).ReturnsAsync(invitation);

            await _handler.Handle(command);

            _invitationRepository.Verify(x => x.Resend(It.Is<Invitation>(c => c.Id == command.Id && c.Status == InvitationStatus.Pending && c.ExpiryDate == DateTimeProvider.Current.UtcNow.Date.AddDays(8))), Times.Once);
        }
    }
}