using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Commands.RemoveTeamMember;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RemoveTeamMember
{
    [TestFixture]
    public class WhenICallRemoveTeamMember
    {
        private Mock<IMembershipRepository> _membershipRepository;
        private RemoveTeamMemberCommandHandler _handler;
        private Mock<IValidator<RemoveTeamMemberCommand>> _validator;
        private RemoveTeamMemberCommand _command;
        private Membership _teamMember;
        private MembershipView _owner;
        private Mock<IMediator> _mediator;

        private const long ExpectedAccountId = 54561561;

        [SetUp]
        public void Setup()
        {
            _teamMember = new Membership
            {
                UserId = 12,
                AccountId = ExpectedAccountId,
                RoleId = (int)Role.Owner
            };

            _owner = new MembershipView
            {
                UserId = 1,
                AccountId = ExpectedAccountId,
                RoleId = (int)Role.Owner
            };

            _command = new RemoveTeamMemberCommand
            {
                UserId = _teamMember.UserId,
                HashedAccountId = "123a",
                ExternalUserId = Guid.NewGuid().ToString()
            };

            _validator = new Mock<IValidator<RemoveTeamMemberCommand>>();

            _validator.Setup(x => x.Validate(It.IsAny<RemoveTeamMemberCommand>())).Returns(new ValidationResult {ValidationDictionary = new Dictionary<string, string>()});

            _mediator = new Mock<IMediator>();
            _membershipRepository = new Mock<IMembershipRepository>();

            _membershipRepository.Setup(x => x.Get(_teamMember.UserId, _teamMember.AccountId)).ReturnsAsync(_teamMember);
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId)).ReturnsAsync(_owner);
            
            _handler = new RemoveTeamMemberCommandHandler(_mediator.Object, _membershipRepository.Object, _validator.Object);
        }

        [Test]
        public async Task ThenMembershipIsRemoved()
        {
            await _handler.Handle(_command);

            _membershipRepository.Verify(x => x.Remove(_command.UserId, ExpectedAccountId), Times.Once);
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
                HashedAccountId = "as123",
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
                HashedAccountId = "",
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
                HashedAccountId = "",
                ExternalUserId = ownerUser.UserRef
            };

            _membershipRepository.Setup(x => x.Get(membership.UserId, membership.AccountId)).ReturnsAsync(membership);
            _membershipRepository.Setup(x => x.GetCaller(ExpectedAccountId, command.ExternalUserId)).ReturnsAsync(ownerMembership);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);

            _membershipRepository.Verify(x => x.Remove(command.UserId, ExpectedAccountId), Times.Never);
        }

        [Test]
        public async Task ThenTheAuditCommandIsCalledWhenTheCommandIsValid()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(_owner.AccountId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("UserId") && y.NewValue.Equals(_teamMember.UserId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("RoleId") && y.NewValue.Equals(_teamMember.RoleId.ToString())) != null)));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"User {_owner.Email} with role {_owner.RoleId} has removed user {_teamMember.UserId} with role {_teamMember.RoleId} from account {_owner.AccountId}"))));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(_owner.AccountId.ToString()) && y.Type.Equals("Account")) != null)));

            _mediator.Verify(x => x.SendAsync(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(_teamMember.UserId.ToString()) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("Membership"))));
        }

    }
}