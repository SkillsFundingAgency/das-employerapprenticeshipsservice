using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Client;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetLatestActivities
{
    public class GetLatestActivitiesQueryHandler : IAsyncRequestHandler<GetLatestActivitiesQuery, GetLatestActivitiesResponse>
    {
        private readonly IActivitiesClient _activitiesClient;
        private readonly IHashingService _hashingService;

        public GetLatestActivitiesQueryHandler(IActivitiesClient activitiesClient, IHashingService hashingService)
        {
            _activitiesClient = activitiesClient;
            _hashingService = hashingService;
        }

        public async Task<GetLatestActivitiesResponse> Handle(GetLatestActivitiesQuery message)
        {
            var accountId = _hashingService.DecodeValue(message.AccountHashedId);
            var result = await _activitiesClient.GetLatestActivities(accountId);

            return new GetLatestActivitiesResponse
            {
                Result = result
            };
        }
    }
}