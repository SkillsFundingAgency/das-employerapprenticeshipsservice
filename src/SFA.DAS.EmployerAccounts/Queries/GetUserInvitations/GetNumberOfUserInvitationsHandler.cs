using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetUserInvitations;

public class GetNumberOfUserInvitationsHandler : IRequestHandler<GetNumberOfUserInvitationsQuery, GetNumberOfUserInvitationsResponse>
{
    private readonly IValidator<GetNumberOfUserInvitationsQuery> _validator;
    private readonly IInvitationRepository _invitationRepository;

    public GetNumberOfUserInvitationsHandler(IValidator<GetNumberOfUserInvitationsQuery> validator, IInvitationRepository invitationRepository)
    {
        _validator = validator;
        _invitationRepository = invitationRepository;
    }

    public async Task<GetNumberOfUserInvitationsResponse> Handle(GetNumberOfUserInvitationsQuery message, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(message);

        if (!result.IsValid())
        {
            throw new InvalidRequestException(result.ValidationDictionary);
        }

        var repositoryValue = await _invitationRepository.GetNumberOfInvites(message.UserId);


        return new GetNumberOfUserInvitationsResponse { NumberOfInvites = repositoryValue };
    }
}