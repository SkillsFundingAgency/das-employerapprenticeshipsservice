using System.Threading;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Commands.AuditCommand;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Commands.DeleteInvitation;

public class DeleteInvitationCommandHandler : IRequestHandler<DeleteInvitationCommand>
{
    private readonly IInvitationRepository _invitationRepository;
    private readonly IMembershipRepository _membershipRepository;
    private readonly IMediator _mediator;
    private readonly DeleteInvitationCommandValidator _validator;

    public DeleteInvitationCommandHandler(IInvitationRepository invitationRepository, IMembershipRepository membershipRepository, IMediator mediator)
    {
        _invitationRepository = invitationRepository ?? throw new ArgumentNullException(nameof(invitationRepository));
        _membershipRepository = membershipRepository ?? throw new ArgumentNullException(nameof(membershipRepository));
        _mediator = mediator;
        _validator = new DeleteInvitationCommandValidator();
    }

    public async Task<Unit> Handle(DeleteInvitationCommand message, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
            throw new InvalidRequestException(validationResult.ValidationDictionary);

        var owner = await _membershipRepository.GetCaller(message.HashedAccountId, message.ExternalUserId);

        if (owner == null || owner.Role != Role.Owner)
            throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You are not an Owner on this Account" } });

        var existing = await _invitationRepository.Get(owner.AccountId, message.Email);

        if (existing == null)
            throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Invitation not found" } });

        if (IsWrongStatusToDelete(existing.Status))
            throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Wrong status to be deleted" } });

        existing.Status = InvitationStatus.Deleted;

        await _invitationRepository.ChangeStatus(existing);


        await _mediator.Send(new CreateAuditCommand
        {
            EasAuditMessage = new AuditMessage
            {
                Category = "DELETED",
                Description = $"Invitation to {message.Email} deleted from account {existing.AccountId}",
                ChangedProperties = new List<PropertyUpdate>
                {
                    new PropertyUpdate {PropertyName = "Status",NewValue = existing.Status.ToString()}
                },
                RelatedEntities = new List<AuditEntity> { new AuditEntity { Id = existing.AccountId.ToString(), Type = "Account" } },
                AffectedEntity = new AuditEntity { Type = "Invitation", Id = existing.Id.ToString() }
            }
        }, cancellationToken);

        return Unit.Value;
    }

    private static bool IsWrongStatusToDelete(InvitationStatus status)
    {
        switch (status)
        {
            case InvitationStatus.Pending:
            case InvitationStatus.Expired:
                return false;
            default:
                return true;
        }
    }
}