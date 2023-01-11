using SFA.DAS.Activities.Client;

namespace SFA.DAS.EmployerAccounts.Queries.GetLatestActivities;

public class GetLatestActivitiesQueryHandler : IAsyncRequestHandler<GetLatestActivitiesQuery, GetLatestActivitiesResponse>
{
    private readonly IActivitiesClient _activitiesClient;

    public GetLatestActivitiesQueryHandler(IActivitiesClient activitiesClient)
    {
        _activitiesClient = activitiesClient;
    }

    public async Task<GetLatestActivitiesResponse> Handle(GetLatestActivitiesQuery message)
    {
        var result = await _activitiesClient.GetLatestActivities(message.AccountId);

        return new GetLatestActivitiesResponse
        {
            Result = result
        };
    }
}