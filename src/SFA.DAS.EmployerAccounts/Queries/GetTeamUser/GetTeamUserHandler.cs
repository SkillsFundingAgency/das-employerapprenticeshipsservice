using System.Threading;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Queries.GetTeamUser;

public class GetTeamUserHandler : IRequestHandler<GetTeamMemberQuery, GetTeamMemberResponse>
{
    private readonly IMembershipRepository _repository;
    private readonly ILog _logger;

    public GetTeamUserHandler(IMembershipRepository repository, ILog logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<GetTeamMemberResponse> Handle(GetTeamMemberQuery message, CancellationToken cancellationToken)
    {
        _logger.Debug($"Getting team member for account hashed ID {message.HashedAccountId} and team member ID {message.TeamMemberId}");
        var member = await _repository.GetCaller(message.HashedAccountId, message.TeamMemberId);

        return new GetTeamMemberResponse
        {
            User = member
        };
    }
}