using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Notification;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.CreateInvitationTests
{
    [TestFixture]
    public class WhenICallCreateInvitation
    {
        private Mock<IInvitationRepository> _invitationRepository;
        private CreateInvitationCommandHandler _handler;
        private CreateInvitationCommand _command;
        private Mock<IAccountTeamRepository> _accountTeamRepository;
        private Mock<IMediator> _mediator;
        private Mock<EmployerApprenticeshipsServiceConfiguration> _configuration;

        [SetUp]
        public void Setup()
        {
            _invitationRepository = new Mock<IInvitationRepository>();
            _accountTeamRepository = new Mock<IAccountTeamRepository>();

            _mediator = new Mock<IMediator>();

            _configuration = new Mock<EmployerApprenticeshipsServiceConfiguration>();

            _handler = new CreateInvitationCommandHandler(_invitationRepository.Object, _accountTeamRepository.Object, _mediator.Object, _configuration.Object);
            _command = new CreateInvitationCommand
            {
                AccountId = 101,
                Email = "test.user@test.local",
                Name = "Test User",
                RoleId = Role.Owner
            };
            DateTimeProvider.Current = new FakeTimeProvider(DateTime.UtcNow);
        }

        [TearDown]
        public void Teardown()
        {
            DateTimeProvider.ResetToDefault();
        }

        [Test]
        public async Task ValidCommandFromAccountOwnerCreatesInvitation()
        {
            _invitationRepository.Setup(x => x.Get(_command.AccountId, _command.Email)).ReturnsAsync(null);
            _accountTeamRepository.Setup(x => x.GetMembership(_command.AccountId, _command.ExternalUserId)).ReturnsAsync(new Membership
            {
                RoleId = (int)Role.Owner
            });

            await _handler.Handle(_command);

            _invitationRepository.Verify(x => x.Create(It.Is<Invitation>(m => m.AccountId == _command.AccountId && m.Email == _command.Email && m.Name == _command.Name && m.Status == InvitationStatus.Pending && m.RoleId == _command.RoleId && m.ExpiryDate == DateTimeProvider.Current.UtcNow.Date.AddDays(8))), Times.Once);
        }

        [Test]
        public void ValidCommandFromNonAccountOwnerDoesNotCreateInvitation()
        {
            _invitationRepository.Setup(x => x.Get(_command.AccountId, _command.Email)).ReturnsAsync(null);
            _accountTeamRepository.Setup(x => x.GetMembership(_command.AccountId, _command.ExternalUserId)).ReturnsAsync(new Membership
            {
                RoleId = 0
            });

            var exception = Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void ValidCommandFromNonAccountUserDoesNotCreateInvitation()
        {
            _invitationRepository.Setup(x => x.Get(_command.AccountId, _command.Email)).ReturnsAsync(null);
            _accountTeamRepository.Setup(x => x.GetMembership(_command.AccountId, _command.ExternalUserId)).ReturnsAsync(null);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void InvalidCommandThrowsException()
        {
            var command = new CreateInvitationCommand();

            var exception = Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(4));
        }

        [Test]
        public void ValidCommandButExistingDoesNotCreateInvitation()
        {
            _invitationRepository.Setup(x => x.Get(_command.AccountId, _command.Email)).ReturnsAsync(new Invitation
            {
                Id = 1,
                AccountId = _command.AccountId,
                Email = _command.Email
            });

            var exception = Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task ThenTheSendNotificationCommandIsInvoked()
        {
            var userId = 1;
            _invitationRepository.Setup(x => x.Get(_command.AccountId, _command.Email)).ReturnsAsync(null);
            _accountTeamRepository.Setup(x => x.GetMembership(_command.AccountId, _command.ExternalUserId)).ReturnsAsync(new Membership
            {
                RoleId = (int)Role.Owner,
                UserId = userId
            });

            //Act
            await _handler.Handle(_command);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<SendNotificationCommand>(c=>c.ForceFormat 
                                                                                && c.UserId.Equals(userId) 
                                                                                && c.Data.RecipientsAddress.Equals(_command.Email)
                                                                                && c.Data.ReplyToAddress.Equals("noreply@sfa.gov.uk")
                                                                                && c.MessageFormat.Equals(MessageFormat.Email))));
        }
    }
}