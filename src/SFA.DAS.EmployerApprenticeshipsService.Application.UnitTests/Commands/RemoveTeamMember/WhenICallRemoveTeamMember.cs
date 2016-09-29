using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RemoveTeamMember;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Commands.RemoveTeamMember
{
    [TestFixture]
    public class WhenICallRemoveTeamMember
    {
        private Mock<IMembershipRepository> _membershipRepository;
        private RemoveTeamMemberCommandHandler _handler;
        private Mock<IValidator<RemoveTeamMemberCommand>> _validator;

        private const long ExpectedAccountId = 54561561;

        [SetUp]
        public void Setup()
        {

            _validator = new Mock<IValidator<RemoveTeamMemberCommand>>();

            _validator.Setup(x => x.Validate(It.IsAny<RemoveTeamMemberCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});

            _membershipRepository = new Mock<IMembershipRepository>();
            _handler = new RemoveTeamMemberCommandHandler(_membershipRepository.Object, _validator.Object);
        }

        [Test]
        public async Task ThenMembershipIsRemoved()
        {

            var membership = new Membership
            {
                UserId = 12,
                AccountId = ExpectedAccountId,
                RoleId = (int)Role.Owner
            };

            var ownerMembership = new MembershipView
            {
                UserId =1,
                AccountId = ExpectedAccountId,
                RoleId = (int)Role.Owner
            };

            var cmd = new RemoveTeamMemberCommand
            {
                UserId = membership.UserId,
                HashedId = "123a",
                ExternalUserId = Guid.NewGuid().ToString()
            };

            _membershipRepository.Setup(x => x.Get(membership.UserId, membership.AccountId)).ReturnsAsync(membership);
            _membershipRepository.Setup(x => x.GetCaller(cmd.HashedId, cmd.ExternalUserId)).ReturnsAsync(ownerMembership);

            await _handler.Handle(cmd);

            _membershipRepository.Verify(x => x.Remove(cmd.UserId, ExpectedAccountId), Times.Once);
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
                HashedId = "as123",
                ExternalUserId = Guid.NewGuid().ToString()
            };

            _membershipRepository.Setup(x => x.Get(membership.UserId, membership.AccountId)).ReturnsAsync(null);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);

            _membershipRepository.Verify(x => x.Remove(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
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
                HashedId = "",
                ExternalUserId = ownerUser.UserRef
            };

            _membershipRepository.Setup(x => x.Get(membership.UserId, membership.AccountId)).ReturnsAsync(membership);
            _membershipRepository.Setup(x => x.GetCaller(ExpectedAccountId, command.ExternalUserId)).ReturnsAsync(nonOwnerMembership);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);

            _membershipRepository.Verify(x => x.Remove(command.UserId, ExpectedAccountId), Times.Never);
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
                HashedId = "",
                ExternalUserId = ownerUser.UserRef
            };

            _membershipRepository.Setup(x => x.Get(membership.UserId, membership.AccountId)).ReturnsAsync(membership);
            _membershipRepository.Setup(x => x.GetCaller(ExpectedAccountId, command.ExternalUserId)).ReturnsAsync(ownerMembership);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);

            _membershipRepository.Verify(x => x.Remove(command.UserId, ExpectedAccountId), Times.Never);
        }
    }
}