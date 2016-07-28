using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.RemoveTeamMember
{
    public class RemoveTeamMemberCommandHandler : AsyncRequestHandler<RemoveTeamMemberCommand>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IAccountTeamRepository _accountTeamRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly RemoveTeamMemberCommandValidator _validator;

        public RemoveTeamMemberCommandHandler(IMembershipRepository membershipRepository, IAccountTeamRepository accountTeamRepository, IUserAccountRepository userAccountRepository)
        {
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            if (accountTeamRepository == null)
                throw new ArgumentNullException(nameof(accountTeamRepository));
            if (userAccountRepository == null)
                throw new ArgumentNullException(nameof(userAccountRepository));
            _membershipRepository = membershipRepository;
            _accountTeamRepository = accountTeamRepository;
            _userAccountRepository = userAccountRepository;
            _validator = new RemoveTeamMemberCommandValidator();
        }

        protected override async Task HandleCore(RemoveTeamMemberCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var existing = await _membershipRepository.Get(message.UserId, message.AccountId);

            if (existing == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not a member of this team" } });

            var owner = await _accountTeamRepository.GetMembership(message.AccountId, message.ExternalUserId);

            if (owner == null || (Role)owner.RoleId != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not an Owner" } });

            var user = await _userAccountRepository.Get(message.UserId);

            if (user?.UserRef == message.ExternalUserId)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You cannot remove yourself" } });

            await _membershipRepository.Remove(message.UserId, message.AccountId);
        }
    }
}