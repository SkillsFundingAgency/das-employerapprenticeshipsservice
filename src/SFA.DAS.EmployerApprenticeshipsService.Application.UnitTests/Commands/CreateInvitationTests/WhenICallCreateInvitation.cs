using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateInvitation;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.SendNotification;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
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
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IMediator> _mediator;
        private Mock<EmployerApprenticeshipsServiceConfiguration> _configuration;
        private Mock<IValidator<CreateInvitationCommand>> _validator;
        private const long ExpectedAccountId = 545641561;
        private const long ExpectedUserId = 521465;
        private const string ExpectedExternalUserId = "someid";
        private const string ExpectedHashedId = "aaa415ss1";
        private const string ExpectedCallerEmail = "test.user@test.local";

        [SetUp]
        public void Setup()
        {
            _invitationRepository = new Mock<IInvitationRepository>();
            _invitationRepository.Setup(x => x.Get(ExpectedAccountId, ExpectedCallerEmail)).ReturnsAsync(null);
            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(ExpectedHashedId, ExpectedExternalUserId)).ReturnsAsync(new MembershipView {AccountId = ExpectedAccountId, UserId = ExpectedUserId});

            _mediator = new Mock<IMediator>();

            _configuration = new Mock<EmployerApprenticeshipsServiceConfiguration>();

            _validator = new Mock<IValidator<CreateInvitationCommand>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<CreateInvitationCommand>())).ReturnsAsync(new ValidationResult());

            _handler = new CreateInvitationCommandHandler(_invitationRepository.Object, _membershipRepository.Object, _mediator.Object, _configuration.Object, _validator.Object);
            _command = new CreateInvitationCommand
            {
                HashedId = ExpectedHashedId,
                Email = ExpectedCallerEmail,
                Name = "Test User",
                RoleId = Role.Owner,
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
        public async Task ValidCommandFromAccountOwnerCreatesInvitation()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _invitationRepository.Verify(x => x.Create(It.Is<Invitation>(m => m.AccountId == ExpectedAccountId && m.Email == _command.Email && m.Name == _command.Name && m.Status == InvitationStatus.Pending && m.RoleId == _command.RoleId && m.ExpiryDate == DateTimeProvider.Current.UtcNow.Date.AddDays(8))), Times.Once);
        }
        
        [Test]
        public void ValidCommandButExistingDoesNotCreateInvitation()
        {
            _invitationRepository.Setup(x => x.Get(ExpectedAccountId, _command.Email)).ReturnsAsync(new Invitation
            {
                Id = 1,
                AccountId = ExpectedAccountId,
                Email = _command.Email
            });

            var exception = Assert.ThrowsAsync<InvalidRequestException>(async () => await _handler.Handle(_command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
        }

        [Test]
        public void ThenAUnauthorizedAccessExecptionIsThrownIfTheValidionResultIsUnauthorized()
        {
            //Arrange
            _validator.Setup(x => x.ValidateAsync(It.IsAny<CreateInvitationCommand>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_command));
        }

        [Test]
        public async Task ThenTheSendNotificationCommandIsInvoked()
        {
            var userId = 1;
            
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedId, _command.ExternalUserId)).ReturnsAsync(new MembershipView
            {
                RoleId = (int)Role.Owner,
                UserId = userId,
                AccountId = ExpectedAccountId
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