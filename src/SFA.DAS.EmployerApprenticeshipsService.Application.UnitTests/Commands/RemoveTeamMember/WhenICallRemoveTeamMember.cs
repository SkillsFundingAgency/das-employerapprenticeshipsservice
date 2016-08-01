using System;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RemoveTeamMember;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.RemoveTeamMember
{
    [TestFixture]
    public class WhenICallRemoveTeamMember
    {
        private Mock<IMembershipRepository> _membershipRepository;
        private RemoveTeamMemberCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _handler = new RemoveTeamMemberCommandHandler(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenMembershipIsRemoved()
        {
            const long accountId = 2;

            var ownerUser = new User
            {
                Id = 2,
                UserRef = Guid.NewGuid().ToString()
            };

            var membership = new Membership
            {
                UserId = 1,
                AccountId = accountId,
                RoleId = (int)Role.Owner
            };

            var ownerMembership = new MembershipView
            {
                UserId = ownerUser.Id,
                AccountId = accountId,
                RoleId = (int)Role.Owner
            };

            var cmd = new RemoveTeamMemberCommand
            {
                UserId = membership.UserId,
                AccountId = membership.AccountId,
                ExternalUserId = ownerUser.UserRef
            };

            _membershipRepository.Setup(x => x.Get(membership.UserId, membership.AccountId)).ReturnsAsync(membership);
            _membershipRepository.Setup(x => x.GetCaller(cmd.AccountId, cmd.ExternalUserId)).ReturnsAsync(ownerMembership);

            await _handler.Handle(cmd);

            _membershipRepository.Verify(x => x.Remove(cmd.UserId, cmd.AccountId), Times.Once);
        }

        [Test]
        public void IfValidationFailsThenMembershipIsNotRemoved()
        {
            var command = new RemoveTeamMemberCommand();

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(3));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "UserId"), Is.Not.Null);
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "AccountId"), Is.Not.Null);
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "ExternalUserId"), Is.Not.Null);

            _membershipRepository.Verify(x => x.Remove(command.UserId, command.AccountId), Times.Never);
        }

        [Test]
        public void IfMembershipNotFoundThenMembershipIsNotRemoved()
        {
            const long accountId = 2;

            var membership = new Membership
            {
                UserId = 1,
                AccountId = accountId,
                RoleId = (int)Role.Owner
            };

            var command = new RemoveTeamMemberCommand
            {
                UserId = membership.UserId,
                AccountId = membership.AccountId,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            _membershipRepository.Setup(x => x.Get(membership.UserId, membership.AccountId)).ReturnsAsync(null);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);

            _membershipRepository.Verify(x => x.Remove(command.UserId, command.AccountId), Times.Never);
        }

        [Test]
        public void IfCallerIsNotOwnerThenMembershipIsNotRemoved()
        {
            const long accountId = 2;

            var ownerUser = new User
            {
                Id = 2,
                UserRef = Guid.NewGuid().ToString()
            };

            var membership = new Membership
            {
                UserId = 1,
                AccountId = accountId,
                RoleId = (int)Role.Owner
            };

            var nonOwnerMembership = new MembershipView
            {
                UserId = ownerUser.Id,
                AccountId = accountId,
                RoleId = (int)Role.Viewer
            };

            var command = new RemoveTeamMemberCommand
            {
                UserId = membership.UserId,
                AccountId = membership.AccountId,
                ExternalUserId = ownerUser.UserRef
            };

            _membershipRepository.Setup(x => x.Get(membership.UserId, membership.AccountId)).ReturnsAsync(membership);
            _membershipRepository.Setup(x => x.GetCaller(command.AccountId, command.ExternalUserId)).ReturnsAsync(nonOwnerMembership);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);

            _membershipRepository.Verify(x => x.Remove(command.UserId, command.AccountId), Times.Never);
        }

        [Test]
        public void IfTryingToRemoveYourselfThenMembershipIsNotRemoved()
        {
            const long accountId = 2;

            var ownerUser = new User
            {
                Id = 2,
                UserRef = Guid.NewGuid().ToString()
            };

            var membership = new Membership
            {
                UserId = ownerUser.Id,
                AccountId = accountId,
                RoleId = (int)Role.Owner
            };

            var ownerMembership = new MembershipView
            {
                UserId = ownerUser.Id,
                AccountId = accountId,
                RoleId = (int)Role.Owner
            };

            var command = new RemoveTeamMemberCommand
            {
                UserId = membership.UserId,
                AccountId = membership.AccountId,
                ExternalUserId = ownerUser.UserRef
            };

            _membershipRepository.Setup(x => x.Get(membership.UserId, membership.AccountId)).ReturnsAsync(membership);
            _membershipRepository.Setup(x => x.GetCaller(command.AccountId, command.ExternalUserId)).ReturnsAsync(ownerMembership);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);

            _membershipRepository.Verify(x => x.Remove(command.UserId, command.AccountId), Times.Never);
        }
    }
}