using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ResendInvitation
{
    public class ResendInvitationCommandHandler : AsyncRequestHandler<ResendInvitationCommand>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly ResendInvitationCommandValidator _validator;

        public ResendInvitationCommandHandler(IInvitationRepository invitationRepository, IMembershipRepository membershipRepository)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _invitationRepository = invitationRepository;
            _membershipRepository = membershipRepository;
            _validator = new ResendInvitationCommandValidator();
        }

        protected override async Task HandleCore(ResendInvitationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var owner = await _membershipRepository.GetCaller(message.AccountId, message.ExternalUserId);

            if (owner == null || (Role)owner.RoleId != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "User is not an Owner" } });

            var existing = await _invitationRepository.Get(message.Id);

            if (existing == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Invitation not found" } });

            if (IsWrongStatusToResend(existing.Status))
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Wrong status to be deleted" } });

            existing.Status = InvitationStatus.Pending;
            existing.ExpiryDate = DateTimeProvider.Current.UtcNow.Date.AddDays(8);

            await _invitationRepository.Resend(existing);
        }

        private bool IsWrongStatusToResend(InvitationStatus status)
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