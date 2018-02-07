using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Client;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetActivities
{
    public class GetActivitiesQueryHandler : IAsyncRequestHandler<GetActivitiesQuery, GetActivitiesResponse>
    {
        private readonly IActivitiesClient _activitiesClient;
        private readonly IHashingService _hashingService;

        public GetActivitiesQueryHandler(IActivitiesClient activitiesClient, IHashingService hashingService)
        {
            _activitiesClient = activitiesClient;
            _hashingService = hashingService;
        }

        public async Task<GetActivitiesResponse> Handle(GetActivitiesQuery message)
        {
            var accountId = _hashingService.DecodeValue(message.AccountHashedId);

            var result = await _activitiesClient.GetActivities(new ActivitiesQuery
            {
                AccountId = accountId,
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