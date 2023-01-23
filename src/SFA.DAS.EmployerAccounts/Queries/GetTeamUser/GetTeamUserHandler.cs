using System.Threading;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.EmployerAccounts.Queries.GetTeamUser;

public class GetTeamUserHandler : IRequestHandler<GetTeamMemberQuery, GetTeamMemberResponse>
{
    private readonly IMembershipRepository _repository;
    private readonly ILogger<GetTeamUserHandler> _logger;

    public GetTeamUserHandler(IMembershipRepository repository, ILogger<GetTeamUserHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<GetTeamMemberResponse> Handle(GetTeamMemberQuery message, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Getting team member for account hashed ID {message.HashedAccountId} and team member ID {message.TeamMemberId}");
        var member = await _repository.GetCaller(message.HashedAccountId, message.TeamMemberId);

        return new GetTeamMemberResponse
        {
            User = member
        };
    }
}