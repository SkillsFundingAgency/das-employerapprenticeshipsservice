using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ChangeTeamMemberRole
{
    public class ChangeTeamMemberRoleCommandHandler : AsyncRequestHandler<ChangeTeamMemberRoleCommand>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly ChangeTeamMemberRoleCommandValidator _validator;

        public ChangeTeamMemberRoleCommandHandler(IMembershipRepository membershipRepository)
        {
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _membershipRepository = membershipRepository;
            _validator = new ChangeTeamMemberRoleCommandValidator();
        }

        protected override async Task HandleCore(ChangeTeamMemberRoleCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var caller = await _membershipRepository.GetCaller(message.HashedId, message.ExternalUserId);

            if (caller == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You are not a member of this Account" } });
            if (caller.RoleId != (int)Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You must be an owner of this Account" } });

            var existing = await _membershipRepository.Get(caller.AccountId, message.Email);

            if (existing == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "Membership not found" } });

            if (caller.UserId == existing.Id)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You cannot change your own role" } });

            await _membershipRepository.ChangeRole(existing.Id, caller.AccountId, message.RoleId);
        }
    }
}