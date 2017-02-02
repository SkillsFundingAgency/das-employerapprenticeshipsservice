using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.GetUserInvitations
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
