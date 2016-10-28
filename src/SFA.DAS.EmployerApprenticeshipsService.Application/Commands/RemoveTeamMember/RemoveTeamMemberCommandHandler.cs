using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Commands.RemoveTeamMember
{
    public class RemoveTeamMemberCommandHandler : AsyncRequestHandler<RemoveTeamMemberCommand>
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IValidator<RemoveTeamMemberCommand> _validator;

        public RemoveTeamMemberCommandHandler(IMembershipRepository membershipRepository, IValidator<RemoveTeamMemberCommand> validator)
        {
            if (membershipRepository == null)
                throw new ArgumentNullException(nameof(membershipRepository));
            _membershipRepository = membershipRepository;
            _validator = validator;
        }

        protected override async Task HandleCore(RemoveTeamMemberCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var owner = await _membershipRepository.GetCaller(message.HashedId, message.ExternalUserId);

            if (owner == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not a member of this Account" } });
            if ((Role)owner.RoleId != Role.Owner)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not an Owner" } });

            var existing = await _membershipRepository.Get(message.UserId, owner.AccountId);

            if (existing == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "User is not a member of this team" } });

            if (message.UserId == owner.UserId)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Membership", "You cannot remove yourself" } });

            await _membershipRepository.Remove(message.UserId, owner.AccountId);
        }
    }
}