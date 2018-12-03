﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserInvitations
{
    public class GetNumberOfUserInvitationsHandler : IAsyncRequestHandler<GetNumberOfUserInvitationsQuery, GetNumberOfUserInvitationsResponse>
    {
        private readonly IValidator<GetNumberOfUserInvitationsQuery> _validator;
        private readonly IInvitationRepository _invitationRepository;

        public GetNumberOfUserInvitationsHandler(IValidator<GetNumberOfUserInvitationsQuery> validator, IInvitationRepository invitationRepository)
        {
            _validator = validator;
            _invitationRepository = invitationRepository;
        }

        public async Task<GetNumberOfUserInvitationsResponse> Handle(GetNumberOfUserInvitationsQuery message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var repositoryValue = await _invitationRepository.GetNumberOfInvites(message.UserId);


            return new GetNumberOfUserInvitationsResponse { NumberOfInvites = repositoryValue };
        }
    }
}
