using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.ChangeTeamMemberRole;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.ChangeTeamMemberRoleTests
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

        [SetUp]
        public void Setup()
        {
            

            _command = new ChangeTeamMemberRoleCommand
            {
                HashedAccountId = "1",
                Email = "test.user@test.local",
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString(),

            };

            _callerMembership = new MembershipView
            {
                AccountId = ExpectedAccountId,
                UserId = 1,
                RoleId = (int)Role.Owner
            };

            _userMembership = new TeamMember
            {
                AccountId = _callerMembership.AccountId,
                Id = _callerMembership.UserId + 1,
                Role = (Role)_command.RoleId
            };

            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId)).ReturnsAsync(_callerMembership);
            _membershipRepository.Setup(x => x.Get(_callerMembership.AccountId, _command.Email)).ReturnsAsync(_userMembership);

            _mediator = new Mock<IMediator>();

            _handler = new ChangeTeamMemberRoleCommandHandler(_membershipRepository.Object, _mediator.Object);
            
        }

        [Test]
        public void IfCallerNotMemberOfAccountWillThrowException()
        {
            var command = new ChangeTeamMemberRoleCommand
            {
                HashedAccountId = "1",
                Email = "test.user@test.local",
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            _membershipRepository.Setup(x => x.GetCaller(command.HashedAccountId, command.ExternalUserId)).ReturnsAsync(null);

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
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var callerMembership = new MembershipView
            {
                AccountId = ExpectedAccountId,
                UserId = 1,
                RoleId = (int)Role.Viewer
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
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var callerMembership = new MembershipView
            {
                AccountId = ExpectedAccountId,
                UserId = 1,
                RoleId = (int)Role.Owner
            };

            _membershipRepository.Setup(x => x.GetCaller(callerMembership.AccountId, command.ExternalUserId)).ReturnsAsync(callerMembership);
            _membershipRepository.Setup(x => x.Get(callerMembership.AccountId, command.Email)).ReturnsAsync(null);

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
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var callerMembership = new MembershipView
            {
                AccountId = ExpectedAccountId,
                UserId = 1,
                RoleId = (int)Role.Owner
            };

            var userMembership = new TeamMember
            {
                AccountId = callerMembership.AccountId,
                Id = callerMembership.UserId,
                Role = (Role)callerMembership.RoleId
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

            _membershipRepository.Verify(x => x.ChangeRole(_userMembership.Id, _callerMembership.AccountId, _command.RoleId), Times.Once);
        }

        [Test]
        public void ThenAnInvalidRequestWillThrowException()
        {
            var command = new ChangeTeamMemberRoleCommand();

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(4));

            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "AccountId"), Is.Not.Null);
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Email"), Is.Not.Null);
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "RoleId"), Is.Not.Null);
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "ExternalUserId"), Is.Not.Null);
        }

        [Test]
        public async Task ThenTheTheCommandIsAuditedIfItIsValid()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("RoleId") && y.NewValue.Equals(_command.RoleId.ToString())) != null 
                    )));
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"Member {_command.Email} on account {ExpectedAccountId} role has changed to {_command.RoleId.ToString()}"))));
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