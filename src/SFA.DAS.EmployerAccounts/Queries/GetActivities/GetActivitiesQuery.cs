using SFA.DAS.Activities;
using SFA.DAS.Authorization.ModelBinding;

namespace SFA.DAS.EmployerAccounts.Queries.GetActivities;

public class GetActivitiesQuery : IAuthorizationContextModel, IAsyncRequest<GetActivitiesResponse>
{
    public long AccountId { get; set; }
    public int? Take { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string Term { get; set; }
    public ActivityTypeCategory? Category { get; set; }
    public Dictionary<string, string> Data { get; set; }
}