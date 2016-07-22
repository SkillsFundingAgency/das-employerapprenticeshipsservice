using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AcceptInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Tests.Commands.AcceptInvitationTests
{
    [TestFixture]
    public class WhenICallAcceptInvitation
    {
        private Mock<IInvitationRepository> _invitationRepository;
        private AcceptInvitationCommandHandler _handler;
        private Invitation _invitation;

        [SetUp]
        public void Setup()
        {
            _invitationRepository = new Mock<IInvitationRepository>();
            _handler = new AcceptInvitationCommandHandler(_invitationRepository.Object);
            _invitation = new Invitation
            {
                Id = 1,
                AccountId = 101,
                Email = "test.user@test.local",
                Status = InvitationStatus.Pending,
                ExpiryDate = DateTimeProvider.Current.UtcNow.AddDays(2)
            };
        }

        [TearDown]
        public void Teardown()
        {
            DateTimeProvider.ResetToDefault();
        }

        [Test]
        public async Task IfExistsAndStatusIsPending()
        {
            _invitationRepository.Setup(x => x.Get(_invitation.Id)).ReturnsAsync(_invitation);

            await _handler.Handle(new AcceptInvitationCommand
            {
                Id = _invitation.Id
            });

            _invitationRepository.Verify(x => x.ChangeStatus(It.Is<Invitation>(z => z.Id == _invitation.Id && z.Status == InvitationStatus.Accepted)), Times.Once);
        }

        [Test]
        public async Task IfNotExists()
        {
            const long invitationId = 1;

            _invitationRepository.Setup(x => x.Get(invitationId)).ReturnsAsync(null);

            await _handler.Handle(new AcceptInvitationCommand
            {
                Id = invitationId
            });

            _invitationRepository.Verify(x => x.ChangeStatus(It.Is<Invitation>(z => z.Id == invitationId && z.Status == InvitationStatus.Accepted)), Times.Never);
        }

        [Test]
        public async Task IfStatusIsNotPending()
        {
            _invitation.Status = InvitationStatus.Accepted;

            _invitationRepository.Setup(x => x.Get(_invitation.Id)).ReturnsAsync(_invitation);

            await _handler.Handle(new AcceptInvitationCommand
            {
                Id = _invitation.Id
            });

            _invitationRepository.Verify(x => x.ChangeStatus(It.Is<Invitation>(z => z.Id == _invitation.Id && z.Status == InvitationStatus.Accepted)), Times.Never);
        }

        [Test]
        public async Task IfHasExpired()
        {
            _invitation.ExpiryDate = DateTimeProvider.Current.UtcNow.AddDays(-2);

            _invitationRepository.Setup(x => x.Get(_invitation.Id)).ReturnsAsync(_invitation);

            await _handler.Handle(new AcceptInvitationCommand
            {
                Id = _invitation.Id
            });

            _invitationRepository.Verify(x => x.ChangeStatus(It.Is<Invitation>(z => z.Id == _invitation.Id && z.Status == InvitationStatus.Accepted)), Times.Never);
        }

        [Test]
        public void IfInvalidRequest()
        {
            var command = new AcceptInvitationCommand();

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }
    }
}