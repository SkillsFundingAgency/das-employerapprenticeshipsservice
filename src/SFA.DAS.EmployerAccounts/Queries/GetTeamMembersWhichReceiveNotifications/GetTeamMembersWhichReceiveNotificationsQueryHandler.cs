using System.Threading;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembersWhichReceiveNotifications;

public class GetTeamMembersWhichReceiveNotificationsQueryHandler : IRequestHandler<GetTeamMembersWhichReceiveNotificationsQuery, GetTeamMembersWhichReceiveNotificationsQueryResponse>
{
    private readonly IEmployerAccountTeamRepository _repository;
    private readonly IValidator<GetTeamMembersWhichReceiveNotificationsQuery> _validator;
    private readonly ILogger<GetTeamMembersWhichReceiveNotificationsQueryHandler> _logger;

    public GetTeamMembersWhichReceiveNotificationsQueryHandler(
        IEmployerAccountTeamRepository repository, 
        IValidator<GetTeamMembersWhichReceiveNotificationsQuery> validator,
        ILogger<GetTeamMembersWhichReceiveNotificationsQueryHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<GetTeamMembersWhichReceiveNotificationsQueryResponse> Handle(GetTeamMembersWhichReceiveNotificationsQuery message, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        _logger.LogInformation("Getting team members for account id {AccountId}", message.AccountId);

        var teamMembers = await _repository.GetAccountTeamMembers(message.AccountId);

        return new GetTeamMembersWhichReceiveNotificationsQueryResponse
        {
            TeamMembersWhichReceiveNotifications = teamMembers
                .Where(p => 
                    p.Status == InvitationStatus.Accepted && 
                    p.CanReceiveNotifications && 
                    p.Role == Models.Role.Owner)
                .ToList()
        };
    }
}