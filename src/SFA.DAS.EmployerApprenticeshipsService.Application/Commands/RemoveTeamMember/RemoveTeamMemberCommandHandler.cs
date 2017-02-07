using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Audit.Types;
using SFA.DAS.EAS.Application.Commands.AuditCommand;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Audit;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Commands.RemoveTeamMember
{
    public class RemoveTeamMemberCommandHandler : AsyncRequestHandler<RemoveTeamMemberCommand>
    {
        private readonly IMediator _mediator;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IValidator<RemoveTeamMemberCommand> _validator;

        public RemoveTeamMemberCommandHandler(IMediator mediator, IMembershipRepository membershipRepository, IValidator<RemoveTeamMemberCommand> validator)
        {
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _mediator = mediator;
            _membershipRepository = membershipRepository;
            _validator = validator;
        }

        protected override async Task HandleCore(RemoveTeamMemberCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);
            
            if (owner == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not a member of this Account" } });
            if ((Role)owner.RoleId != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not an Owner" } });

            var teamMember = await _membershipRepository.Get(message.UserId, owner.AccountId);

            if (teamMember == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not a member of this team" } });

            if (message.UserId == owner.UserId)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You cannot remove yourself" } });

            await AddAuditEntry(owner, teamMember);

            await _membershipRepository.Remove(message.UserId, owner.AccountId);
        }

        private async Task AddAuditEntry(MembershipView owner, Membership teamMember)
        {
            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "DELETED",
                    Description = $"User {owner.Email} with role {owner.RoleId} has removed user {teamMember.UserId} with role {teamMember.RoleId} from account {owner.AccountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        new PropertyUpdate {PropertyName = "AccountId", NewValue = owner.AccountId.ToString()},
                        new PropertyUpdate {PropertyName = "UserId", NewValue = teamMember.UserId.ToString()},
                        new PropertyUpdate {PropertyName = "RoleId", NewValue = teamMember.RoleId.ToString()}
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = owner.AccountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "Membership", Id = teamMember.UserId.ToString() }
                }
            });
        }
    }
}