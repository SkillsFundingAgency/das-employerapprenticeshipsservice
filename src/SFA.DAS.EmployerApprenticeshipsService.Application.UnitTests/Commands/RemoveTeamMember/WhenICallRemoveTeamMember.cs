using System;
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
        private Mock<IAccountTeamRepository> _accountTeamRepository;
        private RemoveTeamMemberCommandHandler _handler;
        private Mock<IUserAccountRepository> _userRepository;

        [SetUp]
        public void Setup()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _accountTeamRepository = new Mock<IAccountTeamRepository>();
            _userRepository = new Mock<IUserAccountRepository>();
            _handler = new RemoveTeamMemberCommandHandler(_membershipRepository.Object, _accountTeamRepository.Object, _userRepository.Object);
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

            var ownerMembership = new Membership
            {
                UserId = ownerUser.Id,
                AccountId = accountId,
                RoleId = (int)Role.Owner
            };

            var cmd = new RemoveTeamMemberCommand
            {
                UserId = membership.UserId,
                AccountId = membership.AccountId,
                ExternalUserId = Guid.NewGuid().ToString()
            };

            _membershipRepository.Setup(x => x.Get(membership.UserId, membership.AccountId)).ReturnsAsync(membership);
            _accountTeamRepository.Setup(x => x.GetMembership(cmd.AccountId, cmd.ExternalUserId)).ReturnsAsync(ownerMembership);
            _userRepository.Setup(x => x.Get(ownerUser.Id)).ReturnsAsync(ownerUser);

            await _handler.Handle(cmd);

            _membershipRepository.Verify(x => x.Remove(cmd.UserId, cmd.AccountId), Times.Once);
        }
    }
}