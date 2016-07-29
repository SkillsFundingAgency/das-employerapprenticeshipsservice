using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ChangeTeamMemberRole;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.ChangeTeamMemberRoleTests
{
    [TestFixture]
    public class WhenICallChangeTeamMemberRole
    {
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<IAccountTeamRepository> _accountTeamRepository;
        private ChangeTeamMemberRoleCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _accountTeamRepository = new Mock<IAccountTeamRepository>();
            _handler = new ChangeTeamMemberRoleCommandHandler(_membershipRepository.Object, _accountTeamRepository.Object);
        }

        [Test]
        public void IfCallerNotMemberOfAccountWillThrowException()
        {
            var command = new ChangeTeamMemberRoleCommand
            {
                AccountId = 1,
                Email = "test.user@test.local",
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            _accountTeamRepository.Setup(x => x.GetMembership(command.AccountId, command.ExternalUserId)).ReturnsAsync(null);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);
        }

        [Test]
        public void IfCallerNotAccountOwnerWillThrowException()
        {
            var command = new ChangeTeamMemberRoleCommand
            {
                AccountId = 1,
                Email = "test.user@test.local",
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var callerMembership = new Membership
            {
                AccountId = command.AccountId,
                UserId = 1,
                RoleId = (int)Role.Viewer
            };

            _accountTeamRepository.Setup(x => x.GetMembership(command.AccountId, command.ExternalUserId)).ReturnsAsync(callerMembership);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);
        }

        [Test]
        public void IfUserNotMemberOfAccountWillThrowException()
        {
            var command = new ChangeTeamMemberRoleCommand
            {
                AccountId = 1,
                Email = "test.user@test.local",
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var callerMembership = new Membership
            {
                AccountId = command.AccountId,
                UserId = 1,
                RoleId = (int)Role.Owner
            };

            _accountTeamRepository.Setup(x => x.GetMembership(command.AccountId, command.ExternalUserId)).ReturnsAsync(callerMembership);
            _membershipRepository.Setup(x => x.Get(command.AccountId, command.Email)).ReturnsAsync(null);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);
        }

        [Test]
        public void IfUserIsCallerWillThrowException()
        {
            var command = new ChangeTeamMemberRoleCommand
            {
                AccountId = 1,
                Email = "test.user@test.local",
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var callerMembership = new Membership
            {
                AccountId = command.AccountId,
                UserId = 1,
                RoleId = (int)Role.Owner
            };

            var userMembership = new TeamMember
            {
                AccountId = callerMembership.AccountId,
                Id = callerMembership.UserId,
                Role = (Role)callerMembership.RoleId
            };

            _accountTeamRepository.Setup(x => x.GetMembership(command.AccountId, command.ExternalUserId)).ReturnsAsync(callerMembership);
            _membershipRepository.Setup(x => x.Get(command.AccountId, command.Email)).ReturnsAsync(userMembership);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);
        }

        [Test]
        public async Task WillChangeMembershipRole()
        {
            var command = new ChangeTeamMemberRoleCommand
            {
                AccountId = 1,
                Email = "test.user@test.local",
                RoleId = 1,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            var callerMembership = new Membership
            {
                AccountId = command.AccountId,
                UserId = 1,
                RoleId = (int)Role.Owner
            };

            var userMembership = new TeamMember
            {
                AccountId = callerMembership.AccountId,
                Id = callerMembership.UserId + 1,
                Role = (Role)command.RoleId
            };

            _accountTeamRepository.Setup(x => x.GetMembership(command.AccountId, command.ExternalUserId)).ReturnsAsync(callerMembership);
            _membershipRepository.Setup(x => x.Get(command.AccountId, command.Email)).ReturnsAsync(userMembership);

            await _handler.Handle(command);

            _membershipRepository.Verify(x => x.ChangeRole(userMembership.Id, command.AccountId, command.RoleId), Times.Once);
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