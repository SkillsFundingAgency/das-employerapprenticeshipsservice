using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.DeleteInvitation
{
    public class DeleteInvitationCommandHandler : AsyncRequestHandler<DeleteInvitationCommand>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly DeleteInvitationCommandValidator _validator;

        public DeleteInvitationCommandHandler(IInvitationRepository invitationRepository, IMembershipRepository membershipRepository)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _invitationRepository = invitationRepository;
            _membershipRepository = membershipRepository;
            _validator = new DeleteInvitationCommandValidator();
        }

        protected override async Task HandleCore(DeleteInvitationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var owner = await _membershipRepository.GetCaller(message.AccountId, message.ExternalUserId);

            if (owner == null || (Role)owner.RoleId != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You are not an Owner on this Account" } });

            var existing = await _invitationRepository.Get(message.AccountId, message.Email);

            if (existing == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Invitation not found" } });

            if (IsWrongStatusToDelete(existing.Status))
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Wrong status to be deleted" } });

            existing.Status = InvitationStatus.Deleted;

            await _invitationRepository.ChangeStatus(existing);
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