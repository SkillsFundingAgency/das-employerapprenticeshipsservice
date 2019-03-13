using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus;
using SFA.DAS.Validation;
using Entity = SFA.DAS.Audit.Types.Entity;
using SFA.DAS.EmployerAccounts.Types.Models;

namespace SFA.DAS.EmployerAccounts.Commands.ChangeTeamMemberRole
{
    public class ChangeTeamMemberRoleCommandHandler : AsyncRequestHandler<ChangeTeamMemberRoleCommand>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;
        private readonly IEventPublisher _eventPublisher;
        private readonly ChangeTeamMemberRoleCommandValidator _validator;

        public ChangeTeamMemberRoleCommandHandler(IMembershipRepository membershipRepository, IMediator mediator, IEventPublisher eventPublisher)
        {
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _membershipRepository = membershipRepository;
            _mediator = mediator;
            _eventPublisher = eventPublisher;
            _validator = new ChangeTeamMemberRoleCommandValidator();
        }

        protected override async Task HandleCore(ChangeTeamMemberRoleCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var caller = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

            if (caller == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You are not a member of this Account" } });
            if (caller.Role != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You must be an owner of this Account" } });

            var existing = await _membershipRepository.Get(caller.AccountId, message.Email);

            if (existing == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "Membership not found" } });

            if (caller.UserId == existing.Id)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You cannot change your own role" } });

            await _membershipRepository.ChangeRole(existing.Id, caller.AccountId, message.Role);

            await _eventPublisher.Publish(new AccountUserRolesUpdatedEvent(caller.AccountId, Guid.Parse(existing.UserRef),
                (UserRole)message.Role, DateTime.UtcNow));

            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "UPDATED",
                    Description = $"Member {message.Email} on account {caller.AccountId} role has changed to {message.Role}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        new PropertyUpdate {PropertyName = "Role",NewValue = message.Role.ToString()}
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = caller.AccountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "Membership", Id = existing.Id.ToString() }
                }
            });
        }
    }
}