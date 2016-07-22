using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.TimeProvider;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AcceptInvitation
{
    public class AcceptInvitationCommandHandler : AsyncRequestHandler<AcceptInvitationCommand>
    {
        private readonly IInvitationRepository _invitationRepository;

        public AcceptInvitationCommandHandler(IInvitationRepository invitationRepository)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            _invitationRepository = invitationRepository;
        }

        protected override async Task HandleCore(AcceptInvitationCommand message)
        {
            //TODO: Validation

            var existing = await _invitationRepository.Get(message.Id);

            if (existing == null)
                return;

            //TODO: What happens if status not Pending and/or expired
            if (existing.Status != InvitationStatus.Pending)
                return;
            if (existing.ExpiryDate < DateTimeProvider.Current.UtcNow)
                return;

            existing.Status = InvitationStatus.Accepted;

            await _invitationRepository.ChangeStatus(existing);
        }
    }
}