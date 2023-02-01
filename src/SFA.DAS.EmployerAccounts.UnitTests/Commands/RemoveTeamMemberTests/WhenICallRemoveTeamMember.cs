using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Commands.RemoveTeamMember;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Models.UserProfile;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.RemoveTeamMemberTests
{
    [TestFixture]
    public class WhenICallRemoveTeamMember
    {
        private Mock<IMembershipRepository> _membershipRepository;
        private RemoveTeamMemberCommandHandler _handler;
        private Mock<IValidator<RemoveTeamMemberCommand>> _validator;
        private RemoveTeamMemberCommand _command;
        private User _user;
        private TeamMember _teamMember;
        private MembershipView _owner;
        private Mock<IMediator> _mediator;
        private Mock<IEventPublisher> _publisher;

        private const long ExpectedAccountId = 54561561;

        [SetUp]
        public void Setup()
        {
            _user = new User { UserRef = Guid.NewGuid().ToString() };

            _teamMember = new TeamMember
            {
                Id = 12,
                AccountId = ExpectedAccountId,
                Role = Role.Owner,
            };

            _owner = new MembershipView
            {
                UserId = 1,
                AccountId = ExpectedAccountId,
                Role = Role.Owner
            };

            _command = new RemoveTeamMemberCommand
            {
                UserId = 12,
                UserRef = Guid.NewGuid(),
                HashedAccountId = "123a",
                ExternalUserId = Guid.NewGuid().ToString()
            };

            _validator = new Mock<IValidator<RemoveTeamMemberCommand>>();

            _validator.Setup(x => x.Validate(It.IsAny<RemoveTeamMemberCommand>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

            _mediator = new Mock<IMediator>();
            _membershipRepository = new Mock<IMembershipRepository>();

            _membershipRepository.Setup(x => x.Get(_teamMember.Id, _teamMember.AccountId)).ReturnsAsync(_teamMember);
            _membershipRepository.Setup(x => x.GetCaller(_command.HashedAccountId, _command.ExternalUserId)).ReturnsAsync(_owner);

            _publisher = new Mock<IEventPublisher>();

            _handler = new RemoveTeamMemberCommandHandler(_mediator.Object, _membershipRepository.Object, _validator.Object, _publisher.Object);
        }

        [Test]
        public async Task ThenMembershipIsRemoved()
        {
            await _handler.Handle(_command, CancellationToken.None);

            _membershipRepository.Verify(x => x.Remove(_command.UserId, ExpectedAccountId), Times.Once);
        }

        [Test]
        public async Task WillPublishUserRoleRemovedEvent()
        {
            await _handler.Handle(_command, CancellationToken.None);

            _publisher.Verify(x => x.Publish(It.Is<AccountUserRemovedEvent>(
                    p => p.AccountId == _teamMember.AccountId &&
                         p.UserRef == _command.UserRef))
                , Times.Once);
        }

        [Test]
        public void IfMembershipNotFoundThenMembershipIsNotRemoved()
        {
            const long accountId = 2;

            var membership = new Membership
            {
                UserId = 1,
                AccountId = accountId,
                Role = Role.Owner
            };

            var command = new RemoveTeamMemberCommand
            {
                UserId = membership.UserId,
                HashedAccountId = "as123",
                ExternalUserId = Guid.NewGuid().ToString()
            };

            _membershipRepository.Setup(x => x.Get(membership.UserId, membership.AccountId)).ReturnsAsync(() => null);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command, CancellationToken.None));

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

            var membership = new TeamMember
            {
                Id = 1,
                AccountId = accountId,
                Role = Role.Owner
            };

            var nonOwnerMembership = new MembershipView
            {
                UserId = ownerUser.Id,
                AccountId = accountId,
                Role = Role.Viewer
            };

            var command = new RemoveTeamMemberCommand
            {
                UserId = membership.Id,
                HashedAccountId = "",
                ExternalUserId = ownerUser.UserRef
            };

            _membershipRepository.Setup(x => x.Get(membership.Id, membership.AccountId)).ReturnsAsync(membership);
            _membershipRepository.Setup(x => x.GetCaller(ExpectedAccountId, command.ExternalUserId)).ReturnsAsync(nonOwnerMembership);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command, CancellationToken.None));

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

            var membership = new TeamMember
            {
                Id = ownerUser.Id,
                AccountId = accountId,
                Role = Role.Owner
            };

            var ownerMembership = new MembershipView
            {
                UserId = ownerUser.Id,
                AccountId = accountId,
                Role = Role.Owner
            };

            var command = new RemoveTeamMemberCommand
            {
                UserId = membership.Id,
                HashedAccountId = "",
                ExternalUserId = ownerUser.UserRef
            };

            _membershipRepository.Setup(x => x.Get(membership.Id, membership.AccountId)).ReturnsAsync(membership);
            _membershipRepository.Setup(x => x.GetCaller(ExpectedAccountId, command.ExternalUserId)).ReturnsAsync(ownerMembership);

            var exception = Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(command, CancellationToken.None));

            Assert.That(exception.ErrorMessages.Count, Is.EqualTo(1));
            Assert.That(exception.ErrorMessages.FirstOrDefault(x => x.Key == "Membership"), Is.Not.Null);

            _membershipRepository.Verify(x => x.Remove(command.UserId, ExpectedAccountId), Times.Never);
        }

        [Test]
        public async Task ThenTheAuditCommandIsCalledWhenTheCommandIsValid()
        {
            //Act
            await _handler.Handle(_command, CancellationToken.None);

            //Assert
            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(_owner.AccountId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("UserId") && y.NewValue.Equals(_teamMember.Id.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Role") && y.NewValue.Equals(_teamMember.Role.ToString())) != null), It.IsAny<CancellationToken>()));

            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"User {_owner.Email} with role {_owner.Role} has removed user {_teamMember.Id} with role {_teamMember.Role} from account {_owner.AccountId}")), It.IsAny<CancellationToken>()));

            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(_owner.AccountId.ToString()) && y.Type.Equals("Account")) != null), It.IsAny<CancellationToken>()));

            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(_teamMember.Id.ToString()) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("Membership")), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenTheAuditPopulatesUserIdWhenUserEmailNotPresent()
        {
            // Arrange
            _user.Email = null;

            //Act
            await _handler.Handle(_command, CancellationToken.None);

            //Assert
            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(_owner.AccountId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("UserId") && y.NewValue.Equals(_teamMember.Id.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Role") && y.NewValue.Equals(_teamMember.Role.ToString())) != null), It.IsAny<CancellationToken>()));

            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"User {_owner.Email} with role {_owner.Role} has removed user {_teamMember.Id} with role {_teamMember.Role} from account {_owner.AccountId}")), It.IsAny<CancellationToken>()));

            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(_owner.AccountId.ToString()) && y.Type.Equals("Account")) != null), It.IsAny<CancellationToken>()));

            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(_teamMember.Id.ToString()) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("Membership")), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task ThenTheAuditPopulatesUserEmailWhenPresent()
        {
            // Arrange
            _teamMember.Email = "test123@test.com";

            //Act
            await _handler.Handle(_command, CancellationToken.None);

            //Assert
            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("AccountId") && y.NewValue.Equals(_owner.AccountId.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("UserId") && y.NewValue.Equals(_teamMember.Id.ToString())) != null &&
                      c.EasAuditMessage.ChangedProperties.SingleOrDefault(y => y.PropertyName.Equals("Role") && y.NewValue.Equals(_teamMember.Role.ToString())) != null), It.IsAny<CancellationToken>()));

            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.Description.Equals($"User {_owner.Email} with role {_owner.Role} has removed user {_teamMember.Email} with role {_teamMember.Role} from account {_owner.AccountId}")), It.IsAny<CancellationToken>()));

            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                      c.EasAuditMessage.RelatedEntities.SingleOrDefault(y => y.Id.Equals(_owner.AccountId.ToString()) && y.Type.Equals("Account")) != null), It.IsAny<CancellationToken>()));

            _mediator.Verify(x => x.Send(It.Is<CreateAuditCommand>(c =>
                    c.EasAuditMessage.AffectedEntity.Id.Equals(_teamMember.Id.ToString()) &&
                    c.EasAuditMessage.AffectedEntity.Type.Equals("Membership")), It.IsAny<CancellationToken>()));
        }
    }
}