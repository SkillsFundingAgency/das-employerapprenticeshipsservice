using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ResendInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.ResendInvitationTests
{
    //TODO Refactor, lots of CTRL C CTRL V
    [TestFixture]
    public class WhenIResendAnInvitation
    {
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IInvitationRepository> _invitationRepository;
        private ResendInvitationCommandHandler _handler;
        private Mock<IMediator> _mediator;
        private EmployerApprenticeshipsServiceConfiguration _config;
        private ResendInvitationCommand _command;
        private const int ExpectedAccountId = 14546;
        private const string ExpectedHashedId = "145AVF46";

        [SetUp]
        public void Setup()
        {
            _command = new ResendInvitationCommand
            {
                Email = "test.user@test.local",
                AccountId = ExpectedHashedId,
                ExternalUserId = Guid.NewGuid().ToString(),
            };
            
            var owner = new MembershipView
            {
                AccountId = ExpectedAccountId,
                UserId = 2,
                RoleId = (int)Role.Owner,
                HashedId = ExpectedHashedId
            };

            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(owner.HashedId, _command.ExternalUserId)).ReturnsAsync(owner);
            _invitationRepository = new Mock<IInvitationRepository>();
            _mediator = new Mock<IMediator>();
            _config = new EmployerApprenticeshipsServiceConfiguration {EmailTemplates = new List<EmailTemplateConfigurationItem> {new EmailTemplateConfigurationItem {Key="Invitation",TemplateName = "Invitation"} } };
            _handler = new ResendInvitationCommandHandler(_invitationRepository.Object, _membershipRepository.Object, _mediator.Object, _config);
        }

        [TearDown]
        public void Teardown()
        {
            DateTimeProvider.ResetToDefault();
        }

        [Test]
        public void InvalidCommandThrowsException()
        {
            //Arrange
            var command = new ResendInvitationCommand();

            //Act
            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));
            
            //Assert
            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(3));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Id"), Is.Not.Null);
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "AccountId"), Is.Not.Null);
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "ExternalUserId"), Is.Not.Null);
        }

        [Test]
        public void CallerIsNotAnAccountOwner()
        {
            //Act
            var exception = Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));

            //Assert
            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);
        }

        [Test]
        public void InvitationDoesNotExist()
        {
            //Arrange
            _invitationRepository.Setup(x => x.Get(ExpectedAccountId, _command.Email)).ReturnsAsync(null);

            //Act
            var exception = Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));

            //Act
            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Invitation"), Is.Not.Null);
        }

        [Test]
        public void AcceptedInvitationsCannotBeResent()
        {
            //Arrange
            var invitation = new Invitation
            {
                Id = 12,
                AccountId = 1,
                Status = InvitationStatus.Accepted
            };
            _invitationRepository.Setup(x => x.Get(ExpectedAccountId, _command.Email)).ReturnsAsync(invitation);

            //Act
            var exception = Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));

            //Assert
            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Invitation"), Is.Not.Null);
        }

        [Test]
        public async Task ShouldResendInvitation()
        {
            //Arrange
            const long invitationId = 12;
            DateTimeProvider.Current = new FakeTimeProvider(DateTime.Now);
            var invitation = new Invitation
            {
                Id = invitationId,
                AccountId = 1,
                Status = InvitationStatus.Deleted,
                ExpiryDate = DateTimeProvider.Current.UtcNow.AddDays(-1)
            };
            _invitationRepository.Setup(x => x.Get(ExpectedAccountId, _command.Email)).ReturnsAsync(invitation);

            //Act
            await _handler.Handle(_command);

            //Assert
            _invitationRepository.Verify(x => x.Resend(It.Is<Invitation>(c => c.Id == invitationId && c.Status == InvitationStatus.Pending && c.ExpiryDate == DateTimeProvider.Current.UtcNow.Date.AddDays(8))), Times.Once);
        }

        [Test]
        public async Task ThenTheSendNotificationCommandIsCalled()
        {
            //Arrange
            var invitation = new Invitation
            {
                Id= 1,
                Email = "test@email",
                AccountId = 1,
                ExpiryDate = DateTimeProvider.Current.UtcNow.AddDays(-1)
            };
            _invitationRepository.Setup(x => x.Get(ExpectedAccountId, _command.Email)).ReturnsAsync(invitation);

            //Act
            await _handler.Handle(_command);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<SendNotificationCommand>(c=> c.Email.RecipientsAddress.Equals(_command.Email)
                                                                                  && c.Email.ReplyToAddress.Equals("noreply@sfa.gov.uk")
                                                                                  && c.Email.SystemId.Equals("x")
                                                                                  && c.Email.Subject.Equals("x")
                                                                                  && c.Email.TemplateId.Equals("Invitation"))));
        }
        
    }
}