using System.Threading;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetTeamMembersWhichReceiveNotifications;

public class GetTeamMembersWhichReceiveNotificationsQueryHandler : IRequestHandler<GetTeamMembersWhichReceiveNotificationsQuery, GetTeamMembersWhichReceiveNotificationsQueryResponse>
{
    private readonly IEmployerAccountTeamRepository _repository;
    private readonly IValidator<GetTeamMembersWhichReceiveNotificationsQuery> _validator;
    private readonly ILog _logger;

    public GetTeamMembersWhichReceiveNotificationsQueryHandler(
        IEmployerAccountTeamRepository repository, 
        IValidator<GetTeamMembersWhichReceiveNotificationsQuery> validator,
        ILog logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<GetTeamMembersWhichReceiveNotificationsQueryResponse> Handle(GetTeamMembersWhichReceiveNotificationsQuery message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        _logger.Info($"Getting team members for account id {message.HashedAccountId}");

        var teamMembers = await _repository.GetAccountTeamMembers(message.HashedAccountId);

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