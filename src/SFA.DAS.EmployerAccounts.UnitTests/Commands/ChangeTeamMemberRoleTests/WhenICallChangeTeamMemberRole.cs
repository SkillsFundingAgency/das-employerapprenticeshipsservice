using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.ChangeTeamMemberRole;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.NServiceBus;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.ChangeTeamMemberRoleTests
{
    //TODO - lots of CTRL C CTRL V here. Refactor!
    [TestFixture]
    public class WhenICallChangeTeamMemberRole
    {
        private const int ExpectedAccountId = 2;
        private Mock<IMembershipRepository> _membershipRepository;
        private ChangeTeamMemberRoleCommandHandler _handler;
        private ChangeTeamMemberRoleCommand _command;
        private MembershipView _callerMembership;
        private TeamMember _userMembership;
        private Mock<IMediator> _mediator;
        private Mock<IEventPublisher> _eventPublisher;

        [SetUp]
        public void Setup()
        {
            

            _command = new ChangeTeamMemberRoleCommand
            {
                HashedAccountId = "1",
                Email = "test.user@test.local",
                Role = Role.Owner,
                ExternalUserId = Guid.NewGuid().ToString(),

            };

            _callerMembership = new MembershipView
            {
                AccountId = ExpectedAccountId,
                UserRef = Guid.NewGuid().ToString(),
                UserId = 1,
                Role = Role.Owner
            };

            _userMembership = new TeamMember
            {
                AccountId = _callerMembership.AccountId,
                UserRef = Guid.NewGuid().ToString(),
                Id = _callerMembership.UserId + 1,
                Role = _command.Role
            };

            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId)).ReturnsAsync(_callerMembership);
            _membershipRepository.Setup(x => x.Get(_callerMembership.AccountId, _command.Email)).ReturnsAsync(_userMembership);

            _mediator = new Mock<IMediator>();
            _eventPublisher = new Mock<IEventPublisher>();

            _handler = new ChangeTeamMemberRoleCommandHandler(_membershipRepository.Object, _mediator.Object, _eventPublisher.Object);
            
        }

        [Test]
        public void IfCallerNotMemberOfAccountWillThrowException()
        {
            var command = new ChangeTeamMemberRoleCommand
            {
                HashedAccountId = "1",
                Email = "test.user@test.local",
                Role = Role.Owner,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            _membershipRepository.Setup(x => x.GetCaller(command.HashedAccountId, command.ExternalUserId)).ReturnsAsync(() => null);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);
        }

        [Test]
        public void IfCallerNotAccountOwnerWillThrowException()
        {
            var command = new ChangeTeamMemberRoleCommand
            {
                HashedAccountId = "1",
                Email = "test.user@test.local",
                Role = Role.Owner,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var callerMembership = new MembershipView
            {
                AccountId = ExpectedAccountId,
                UserId = 1,
                Role = Role.Viewer
            };

            _membershipRepository.Setup(x => x.GetCaller(callerMembership.AccountId, command.ExternalUserId)).ReturnsAsync(callerMembership);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);
        }

        [Test]
        public void IfUserNotMemberOfAccountWillThrowException()
        {
            var command = new ChangeTeamMemberRoleCommand
            {
                HashedAccountId = "1",
                Email = "test.user@test.local",
                Role = Role.Owner,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var callerMembership = new MembershipView
            {
                AccountId = ExpectedAccountId,
                UserId = 1,
                Role = Role.Owner
            };

            _membershipRepository.Setup(x => x.GetCaller(callerMembership.AccountId, command.ExternalUserId)).ReturnsAsync(callerMembership);
            _membershipRepository.Setup(x => x.Get(callerMembership.AccountId, command.Email)).ReturnsAsync(() => null);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);
        }

        [Test]
        public void IfUserIsCallerWillThrowException()
        {
            var command = new ChangeTeamMemberRoleCommand
            {
                HashedAccountId = "1",
                Email = "test.user@test.local",
                Role = Role.Owner,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var callerMembership = new MembershipView
            {
                AccountId = ExpectedAccountId,
                UserId = 1,
                Role = Role.Owner
            };

            var userMembership = new TeamMember
            {
                AccountId = callerMembership.AccountId,
                Id = callerMembership.UserId,
                Role = callerMembership.Role
            };

            _membershipRepository.Setup(x => x.GetCaller(callerMembership.AccountId, command.ExternalUserId)).ReturnsAsync(callerMembership);
            _membershipRepository.Setup(x => x.Get(callerMembership.AccountId, command.Email)).ReturnsAsync(userMembership);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);
        }

        [Test]
        public async Task WillChangeMembershipRole()
        {
            await _handler.Handle(_command);

            _membershipRepository.Verify(x => x.ChangeRole(_userMembership.Id, _callerMembership.AccountId, _command.Role), Times.Once);
        }

        [Test]
        public async Task WillPublishUserRoleUpdatedEvent()
        {
            await _handler.Handle(_command);

            _eventPublisher.Verify(x => x.Publish(It.Is<AccountUserRolesUpdatedEvent>(
                p => p.AccountId  == _userMembership.AccountId &&
                     p.UserRef.ToString() == _userMembership.UserRef &&
                     p.Role == (UserRole)_command.Role))
                , Times.Once);
        }

        [Test]
        public void ThenAnInvalidRequestWillThrowException()
        {
            var command = new ChangeTeamMemberRoleCommand();

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(4));

            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "AccountId"), Is.Not.Null);
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Email"), Is.Not.Null);
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Role"), Is.Not.Null);
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "ExternalUserId"), Is.Not.Null);
        }

        [Test]
        public async Task ThenTheTheCommandIsAuditedIfItIsValid()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Role") && y.NewValue.Equals(_command.Role.ToString())) != null 
                    )));
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"Member {_command.Email} on account {ExpectedAccountId} role has changed to {_command.Role.ToString()}"))));
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(ExpectedAccountId.ToString()) && y.Type.Equals("Account")) != null
                    )));
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(_userMembership.Id.ToString()) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("Membership")
                    )));
        }
    }
}