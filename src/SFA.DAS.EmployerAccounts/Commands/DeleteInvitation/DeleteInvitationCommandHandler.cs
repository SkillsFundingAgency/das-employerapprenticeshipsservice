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
using SFA.DAS.Validation;
using Entity = SFA.DAS.Audit.Types.Entity;

namespace SFA.DAS.EmployerAccounts.Commands.DeleteInvitation
{
    public class DeleteInvitationCommandHandler : AsyncRequestHandler<DeleteInvitationCommand>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IMediator _mediator;
        private readonly DeleteInvitationCommandValidator _validator;

        public DeleteInvitationCommandHandler(IInvitationRepository invitationRepository, IMembershipRepository membershipRepository, IMediator mediator)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _invitationRepository = invitationRepository;
            _membershipRepository = membershipRepository;
            _mediator = mediator;
            _validator = new DeleteInvitationCommandValidator();
        }

        protected override async Task HandleCore(DeleteInvitationCommand message)
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


            await _mediator.SendAsync(new CreateAuditCommand
            {
                EasAuditMessage = new EasAuditMessage
                {
                    Category = "DELETED",
                    Description = $"Invitation to {message.Email} deleted from account {existing.AccountId}",
                    ChangedProperties = new List<PropertyUpdate>
                    {
                        new PropertyUpdate {PropertyName = "Status",NewValue = existing.Status.ToString()}
                    },
                    RelatedEntities = new List<Entity> { new Entity { Id = existing.AccountId.ToString(), Type = "Account" } },
                    AffectedEntity = new Entity { Type = "Invitation", Id = existing.Id.ToString() }
                }
            });


        }

        private bool IsWrongStatusToDelete(InvitationStatus status)
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
}