using System.Threading;
using SFA.DAS.Activities.Client;

namespace SFA.DAS.EmployerAccounts.Queries.GetLatestActivities;

public class GetLatestActivitiesQueryHandler : IRequestHandler<GetLatestActivitiesQuery, GetLatestActivitiesResponse>
{
    private readonly IActivitiesClient _activitiesClient;

    public GetLatestActivitiesQueryHandler(IActivitiesClient activitiesClient)
    {
        _activitiesClient = activitiesClient;
    }

    public async Task<GetLatestActivitiesResponse> Handle(GetLatestActivitiesQuery message, CancellationToken cancellationToken)
    {
        var result = await _activitiesClient.GetLatestActivities(message.AccountId);

        return new GetLatestActivitiesResponse
        {
            Result = result
        };
    }
}