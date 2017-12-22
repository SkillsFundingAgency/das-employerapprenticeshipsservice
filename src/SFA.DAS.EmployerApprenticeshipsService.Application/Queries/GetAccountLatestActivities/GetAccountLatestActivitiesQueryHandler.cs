using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Client;
using SFA.DAS.EAS.Application.Validation;

namespace SFA.DAS.EAS.Application.Queries.GetAccountLatestActivities
{
    public class GetAccountLatestActivitiesQueryHandler : IAsyncRequestHandler<GetAccountLatestActivitiesQuery, GetAccountLatestActivitiesResponse>
    {
        private readonly IActivitiesClient _activitiesClient;
        private readonly IValidator<GetAccountLatestActivitiesQuery> _validator;

        public GetAccountLatestActivitiesQueryHandler(IActivitiesClient activitiesClient, IValidator<GetAccountLatestActivitiesQuery> validator)
        {
            _activitiesClient = activitiesClient;
            _validator = validator;
        }

        public async Task<GetAccountLatestActivitiesResponse> Handle(GetAccountLatestActivitiesQuery message)
        {
            ValidateMessage(message);

            var result = await _activitiesClient.GetLatestActivities(message.AccountId);

            return new GetAccountLatestActivitiesResponse
            {
                Result = result
            };
        }

        private void ValidateMessage(GetAccountLatestActivitiesQuery message)
        {
            var validationResults = _validator.Validate(message);

            if (!validationResults.IsValid())
            {
                throw new InvalidRequestException(validationResults.ValidationDictionary);
            }
        }
    }
}