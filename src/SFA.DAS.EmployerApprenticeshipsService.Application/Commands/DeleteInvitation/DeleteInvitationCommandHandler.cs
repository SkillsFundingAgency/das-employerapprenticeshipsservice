using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.DeleteInvitation
{
    public class DeleteInvitationCommandHandler : AsyncRequestHandler<DeleteInvitationCommand>
    {
        private readonly IInvitationRepository _invitationRepository;
        private readonly IAccountTeamRepository _accountTeamRepository;
        private readonly DeleteInvitationCommandValidator _validator;

        public DeleteInvitationCommandHandler(IInvitationRepository invitationRepository, IAccountTeamRepository accountTeamRepository)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            if (accountTeamRepository == null)
                throw new ArgumentNullException(nameof(accountTeamRepository));
            _invitationRepository = invitationRepository;
            _accountTeamRepository = accountTeamRepository;
            _validator = new DeleteInvitationCommandValidator();
        }

        protected override async Task HandleCore(DeleteInvitationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var existing = await _invitationRepository.Get(message.Id);

            if (existing == null)
                return;

            var owner = await _accountTeamRepository.GetMembership(message.AccountId, message.ExternalUserId);

            if (owner == null || (Role)owner.RoleId != Role.Owner)
                return;

            existing.Status = InvitationStatus.Deleted;

            await _invitationRepository.ChangeStatus(existing);
        }
    }
}