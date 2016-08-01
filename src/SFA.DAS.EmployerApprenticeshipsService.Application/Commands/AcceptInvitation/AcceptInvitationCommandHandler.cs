using System;
using System.Collections.Generic;
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
        private readonly IMembershipRepository _membershipRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly AcceptInvitationCommandValidator _validator;

        public AcceptInvitationCommandHandler(IInvitationRepository invitationRepository, IMembershipRepository membershipRepository, IUserAccountRepository userAccountRepository)
        {
            if (invitationRepository == null)
                throw new ArgumentNullException(nameof(invitationRepository));
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            if (userAccountRepository == null)
                throw new ArgumentNullException(nameof(userAccountRepository));
            _invitationRepository = invitationRepository;
            _membershipRepository = membershipRepository;
            _userAccountRepository = userAccountRepository;
            _validator = new AcceptInvitationCommandValidator();
        }

        protected override async Task HandleCore(AcceptInvitationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var existing = await _invitationRepository.Get(message.Id);

            if (existing == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Invitation not found" } });

            var user = await _userAccountRepository.Get(existing.Email);

            if (user == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "User", "User not found" } });

            var membership = await _membershipRepository.GetCaller(existing.AccountId, user.UserRef);

            if (membership != null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User already member of Account" } });

            if (existing.Status != InvitationStatus.Pending)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Invitation is not pending" } });
            if (existing.ExpiryDate < DateTimeProvider.Current.UtcNow)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Invitation", "Invitation has expired" } });

            existing.Status = InvitationStatus.Accepted;

            await _invitationRepository.ChangeStatus(existing);
        }
    }
}