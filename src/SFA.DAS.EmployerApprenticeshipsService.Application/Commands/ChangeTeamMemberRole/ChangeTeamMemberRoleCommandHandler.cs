using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Models.Audit;

namespace SFA.DAS.EAS.Application.Commands.ChangeTeamMemberRole
{
    public class ChangeTeamMemberRoleCommandHandler : AsyncRequestHandler<ChangeTeamMemberRoleCommand>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;
        private readonly ChangeTeamMemberRoleCommandValidator _validator;

        public ChangeTeamMemberRoleCommandHandler(IMembershipRepository membershipRepository, IMediator mediator)
        {
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _membershipRepository = membershipRepository;
            _mediator = mediator;
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
            if (caller.RoleId != (int)Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You must be an owner of this Account" } });

            var existing = await _membershipRepository.Get(caller.AccountId, message.Email);

            if (existing == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "Membership not found" } });

            if (caller.UserId == existing.Id)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You cannot change your own role" } });

            await _membershipRepository.ChangeRole(existing.Id, caller.AccountId, message.RoleId);

            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Description = $"Member {message.Email} on account {caller.AccountId} role has changed to {message.RoleId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        new PropertyUpdate {PropertyName = "RoleId",NewValue = message.RoleId.ToString()}
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = caller.AccountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "Membership", Id = existing.Id.ToString() }
                }
            });
        }
    }
}