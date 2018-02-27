using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Client;

namespace SFA.DAS.EAS.Application.Queries.GetActivities
{
    public class GetActivitiesQueryHandler : IAsyncRequestHandler<GetActivitiesQuery, GetActivitiesResponse>
    {
        private readonly IActivitiesClient _activitiesClient;

        public GetActivitiesQueryHandler(IActivitiesClient activitiesClient)
        {
            _activitiesClient = activitiesClient;
        }

        public async Task<GetActivitiesResponse> Handle(GetActivitiesQuery message)
        {
            var result = await _activitiesClient.GetActivities(new ActivitiesQuery
            {
                AccountId = message.AccountId.Value,
                Take = message.Take,
                From = message.From,
                To = message.To,
                Term = message.Term,
                Category = message.Category,
                Data = message.Data
            });

            return new GetActivitiesResponse
            {
                Result = result
            };
        }
    }
}