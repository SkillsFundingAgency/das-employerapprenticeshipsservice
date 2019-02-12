using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.CreateInvitation;
using SFA.DAS.EmployerAccounts.Commands.SendNotification;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.EmployerAccounts.UnitTests.Fakes;
using SFA.DAS.NServiceBus;
using SFA.DAS.TimeProvider;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.CreateInvitationTests
{
    [TestFixture]
    public class WhenICallCreateInvitation
    {
        private Mock<IInvitationRepository> _invitationRepository;
        private CreateInvitationCommandHandler _handler;
        private CreateInvitationCommand _command;
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IMediator> _mediator;
        private EmployerAccountsConfiguration _configuration;
        private Mock<IValidator<CreateInvitationCommand>> _validator;
        private Mock<IUserAccountRepository> _userRepository;
        private Mock<IEventPublisher> _endpoint;
        private const long ExpectedAccountId = 545641561;
        private const long ExpectedUserId = 521465;
        private const long ExpectedInvitationId = 1231234;
        private const string ExpectedHashedId = "aaa415ss1";
        private const string ExpectedCallerEmail = "test.user@test.local";
        private const string ExpectedExistingUserEmail = "registered@test.local";
        private const string NameOfPersonBeingInvited = "Invited User";


        private static readonly string ExpectedExternalUserId = Guid.NewGuid().ToString();

        [SetUp]
        public void Setup()
        {
            _invitationRepository = new Mock<IInvitationRepository>();
            _invitationRepository.Setup(x => x.Get(ExpectedAccountId, ExpectedCallerEmail)).ReturnsAsync(() => null);
            _invitationRepository.Setup(x => x.Create(It.IsAny<Invitation>())).ReturnsAsync(ExpectedInvitationId);

            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(ExpectedHashedId, ExpectedExternalUserId))
                                 .ReturnsAsync(new MembershipView
                                 {
                                     AccountId = ExpectedAccountId,
                                     UserId = ExpectedUserId,
                                     UserRef = ExpectedExternalUserId
                                 });

            _userRepository = new Mock<IUserAccountRepository>();
            _userRepository.Setup(x => x.Get(ExpectedExistingUserEmail)).ReturnsAsync(new User { Email = ExpectedExistingUserEmail, UserRef = Guid.NewGuid().ToString() });

            _endpoint = new Mock<IEventPublisher>();

            _mediator = new Mock<IMediator>();

            _validator = new Mock<IValidator<CreateInvitationCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<CreateInvitationCommand>())).ReturnsAsync(new ValidationResult());

            _configuration = new EmployerAccountsConfiguration();

            _handler = new CreateInvitationCommandHandler(_invitationRepository.Object, _membershipRepository.Object, _mediator.Object, _configuration, _validator.Object, _userRepository.Object, _endpoint.Object);
            _command = new CreateInvitationCommand
            {
                HashedAccountId = ExpectedHashedId,
                EmailOfPersonBeingInvited = ExpectedCallerEmail,
                NameOfPersonBeingInvited = NameOfPersonBeingInvited,
                RoleOfPersonBeingInvited = Role.Owner,
                ExternalUserId = ExpectedExternalUserId
            };
            DateTimeProvider.Current = new FakeTimeProvider(DateTime.UtcNow);
        }

        [TearDown]
        public void Teardown()
        {
            DateTimeProvider.ResetToDefault();
        }

        [Test]
        public async Task Then_InvitedUserEventPublishedWithCorrectPersonInvited()
        {
            //Act
            await _handler.Handle(_command);

            _endpoint.Verify(e =>e.Publish(It.Is<InvitedUserEvent>(i => i.PersonInvited == NameOfPersonBeingInvited)));
        }

        [Test]
        public async Task ValidCommandFromAccountOwnerCreatesInvitation()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _invitationRepository.Verify(x => x.Create(It.Is<Invitation>(m => m.AccountId == ExpectedAccountId && m.Email == _command.EmailOfPersonBeingInvited && m.Name == _command.NameOfPersonBeingInvited && m.Status == InvitationStatus.Pending && m.Role == _command.RoleOfPersonBeingInvited && m.ExpiryDate == DateTimeProvider.Current.UtcNow.Date.AddDays(8))), Times.Once);
        }

        [Test]
        public void ValidCommandButExistingDoesNotCreateInvitation()
        {
            _invitationRepository.Setup(x => x.Get(ExpectedAccountId, _command.EmailOfPersonBeingInvited)).ReturnsAsync(new Invitation
            {
                Id = 1,
                AccountId = ExpectedAccountId,
                Email = _command.EmailOfPersonBeingInvited
            });

            var exception = Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void ThenAUnauthorizedAccessExecptionIsThrownIfTheValidionResultIsUnauthorized()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<CreateInvitationCommand>())).ReturnsAsync(new ValidationResult { IsUnauthorized = true });

            //Act
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_command));
        }

        [Test]
        public async Task ThenTheSendNotificationCommandIsInvoked()
        {
            var userId = 1;

            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId)).ReturnsAsync(new MembershipView
            {
                Role = Role.Owner,
                UserId = userId,
                UserRef = ExpectedExternalUserId,
                AccountId = ExpectedAccountId
            });

            //Act
            await _handler.Handle(_command);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<SendNotificationCommand>(c => c.Email.RecipientsAddress.Equals(ExpectedCallerEmail)
                                                                                  && c.Email.ReplyToAddress.Equals("noreply@sfa.gov.uk")
                                                                                  && c.Email.SystemId.Equals("x")
                                                                                  && c.Email.TemplateId.Equals("InvitationNewUser")
                                                                                  && c.Email.Subject.Equals("x"))));
        }

        [Test]
        public async Task ThenADifferentEmailIsSentIfTheEmailIsAlreadyRegisteredInTheSystem()
        {
            //Arrange
            _command.EmailOfPersonBeingInvited = ExpectedExistingUserEmail;

            //Act
            await _handler.Handle(_command);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<SendNotificationCommand>(c => c.Email.RecipientsAddress.Equals(ExpectedExistingUserEmail)
                                                                                  && c.Email.ReplyToAddress.Equals("noreply@sfa.gov.uk")
                                                                                  && c.Email.SystemId.Equals("x")
                                                                                  && c.Email.TemplateId.Equals("InvitationExistingUser")
                                                                                  && c.Email.Subject.Equals("x"))));
        }

        [Test]
        public async Task ThenTheAuditCommandIsCalledWhenTheCreateInvitationCommandIsValid()
        {
            //Arrange
            var userId = 1;

            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId))
                .ReturnsAsync(new MembershipView
                {
                    Role = Role.Owner,
                    UserId = userId,
                    UserRef = ExpectedExternalUserId,
                    AccountId = ExpectedAccountId
                });

            //Act
            await _handler.Handle(_command);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(ExpectedAccountId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Email") && y.NewValue.Equals(ExpectedCallerEmail)) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Name") && y.NewValue.Equals(_command.NameOfPersonBeingInvited.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Role") && y.NewValue.Equals(_command.RoleOfPersonBeingInvited.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Status") && y.NewValue.Equals(InvitationStatus.Pending.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("ExpiryDate") && y.NewValue.Equals(DateTimeProvider.Current.UtcNow.Date.AddDays(8).ToString("yyyy-MM-dd HH:mm:ss.fffff"))) != null
                    )));
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"Member {ExpectedCallerEmail} added to account {ExpectedAccountId} as {_command.RoleOfPersonBeingInvited.ToString()}"))));
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(ExpectedAccountId.ToString()) && y.Type.Equals("Account")) != null
                    )));
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(ExpectedInvitationId.ToString()) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("Invitation")
                    )));
        }
    }
}