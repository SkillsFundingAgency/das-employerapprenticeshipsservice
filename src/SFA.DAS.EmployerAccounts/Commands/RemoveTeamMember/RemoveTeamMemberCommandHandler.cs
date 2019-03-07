using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.NServiceBus;
using SFA.DAS.Validation;
using Entity = SFA.DAS.Audit.Types.Entity;

namespace SFA.DAS.EmployerAccounts.Commands.RemoveTeamMember
{
    public class RemoveTeamMemberCommandHandler : AsyncRequestHandler<RemoveTeamMemberCommand>
    {
        private readonly IMediator _mediator;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IValidator<RemoveTeamMemberCommand> _validator;
        private readonly IEventPublisher _eventPublisher;

        public RemoveTeamMemberCommandHandler(IMediator mediator, IMembershipRepository membershipRepository, IValidator<RemoveTeamMemberCommand> validator, IEventPublisher eventPublisher)
        {
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _mediator = mediator;
            _membershipRepository = membershipRepository;
            _validator = validator;
            _eventPublisher = eventPublisher;
        }

        protected override async Task HandleCore(RemoveTeamMemberCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);
            
            if (owner == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not a member of this Account" } });
            if (owner.Role != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not an Owner" } });

            var teamMember = await _membershipRepository.Get(message.UserId, owner.AccountId);

            if (teamMember == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not a member of this team" } });

            if (message.UserId == owner.UserId)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You cannot remove yourself" } });

            await AddAuditEntry(owner, teamMember);

            await _membershipRepository.Remove(message.UserId, owner.AccountId);

            await _eventPublisher.Publish(new AccountUserRemovedEvent(teamMember.AccountId, message.UserRef, DateTime.UtcNow));
        }

        private async Task AddAuditEntry(MembershipView owner, Membership teamMember)
        {
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "DELETED",
                    Description = $"User {owner.Email} with role {owner.Role} has removed user {teamMember.UserId} with role {teamMember.Role} from account {owner.AccountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        new PropertyUpdate {PropertyName = "AccountId", NewValue = owner.AccountId.ToString()},
                        new PropertyUpdate {PropertyName = "UserId", NewValue = teamMember.UserId.ToString()},
                        new PropertyUpdate {PropertyName = "Role", NewValue = teamMember.Role.ToString()}
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = owner.AccountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "Membership", Id = teamMember.UserId.ToString() }
                }
            });
        }
    }
}