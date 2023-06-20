using System.Threading;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.NServiceBus.Services;

namespace SFA.DAS.EmployerAccounts.Commands.RemoveTeamMember;

public class RemoveTeamMemberCommandHandler : IRequestHandler<RemoveTeamMemberCommand>
{
    private readonly IMediator _mediator;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IValidator<RemoveTeamMemberCommand> _validator;
    private readonly IEventPublisher _eventPublisher;

    public RemoveTeamMemberCommandHandler(IMediator mediator, IMembershipRepository membershipRepository, IValidator<RemoveTeamMemberCommand> validator, IEventPublisher eventPublisher)
    {
        _mediator = mediator;
        _membershipRepository = membershipRepository ?? throw new ArgumentNullException(nameof(membershipRepository));
        _validator = validator;
        _eventPublisher = eventPublisher;
    }

    public async Task<Unit> Handle(RemoveTeamMemberCommand message, CancellationToken cancellationToken)
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

        return Unit.Value;
    }

    private async Task AddAuditEntry(MembershipView owner, TeamMember teamMember)
    {
        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "DELETED",
                Description = $"User {owner.Email} with role {owner.Role} has removed user {teamMember.Email ?? teamMember.Id.ToString()} with role {teamMember.Role} from account {owner.AccountId}",
                ChangedProperties = new List<PropertyUpdate>
                {
                    new PropertyUpdate {PropertyName = "AccountId", NewValue = owner.AccountId.ToString()},
                    new PropertyUpdate {PropertyName = "UserId", NewValue = teamMember.Id.ToString()},
                    new PropertyUpdate {PropertyName = "Role", NewValue = teamMember.Role.ToString()}
                },
                RelatedEntities = new List<AuditEntity> { new AuditEntity { Id = owner.AccountId.ToString(), Type = "Account" } },
                AffectedEntity = new AuditEntity { Type = "Membership", Id = teamMember.Id.ToString() }
            }
        });
    }
}