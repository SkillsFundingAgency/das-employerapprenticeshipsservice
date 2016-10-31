using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.ChangeTeamMemberRole;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.ChangeTeamMemberRoleTests
{
    //TODO - lots of CTRL C CTRL V here. Refactor!
    [TestFixture]
    public class WhenICallChangeTeamMemberRole
    {
        private Mock<IMembershipRepository> _membershipRepository;
        private ChangeTeamMemberRoleCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _handler = new ChangeTeamMemberRoleCommandHandler(_membershipRepository.Object);
        }

        [Test]
        public void IfCallerNotMemberOfAccountWillThrowException()
        {
            var command = new ChangeTeamMemberRoleCommand
            {
                HashedId = "1",
                Email = "test.user@test.local",
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            _membershipRepository.Setup(x => x.GetCaller(command.HashedId, command.ExternalUserId)).ReturnsAsync(null);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);
        }

        [Test]
        public void IfCallerNotAccountOwnerWillThrowException()
        {
            var command = new ChangeTeamMemberRoleCommand
            {
                HashedId = "1",
                Email = "test.user@test.local",
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var callerMembership = new MembershipView
            {
                AccountId = 2,
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
                HashedId = "1",
                Email = "test.user@test.local",
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var callerMembership = new MembershipView
            {
                AccountId = 2,
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
                HashedId = "1",
                Email = "test.user@test.local",
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var callerMembership = new MembershipView
            {
                AccountId = 2,
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
            var command = new ChangeTeamMemberRoleCommand
            {
                HashedId = "1",
                Email = "test.user@test.local",
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString(),
                
            };

            var callerMembership = new MembershipView
            {
                AccountId = 2,
                UserId = 1,
                RoleId = (int)Role.Owner
            };

            var userMembership = new TeamMember
            {
                AccountId = callerMembership.AccountId,
                Id = callerMembership.UserId + 1,
                Role = (Role)command.RoleId
            };
            
            _membershipRepository.Setup(x => x.GetCaller(command.HashedId, command.ExternalUserId)).ReturnsAsync(callerMembership);
            _membershipRepository.Setup(x => x.Get(callerMembership.AccountId, command.Email)).ReturnsAsync(userMembership);

            await _handler.Handle(command);

            _membershipRepository.Verify(x => x.ChangeRole(userMembership.Id, callerMembership.AccountId, command.RoleId), Times.Once);
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
    }
}