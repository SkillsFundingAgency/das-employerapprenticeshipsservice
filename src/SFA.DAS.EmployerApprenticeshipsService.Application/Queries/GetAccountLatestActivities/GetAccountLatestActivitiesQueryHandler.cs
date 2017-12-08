using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Client;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Queries.GetAccountLatestActivities
{
    public class GetAccountLatestActivitiesQueryHandler : IAsyncRequestHandler<GetAccountLatestActivitiesQuery, GetAccountLatestActivitiesResponse>
    {
        private readonly IActivitiesClient _activitiesClient;
        private readonly ILog _logger;
        private readonly IValidator<GetAccountLatestActivitiesQuery> _validator;

        public GetAccountLatestActivitiesQueryHandler(IActivitiesClient activitiesClient, ILog logger, IValidator<GetAccountLatestActivitiesQuery> validator)
        {
            _activitiesClient = activitiesClient;
            _logger = logger;
            _validator = validator;
        }

        public async Task<GetAccountLatestActivitiesResponse> Handle(GetAccountLatestActivitiesQuery message)
        {
            ValidateMessage(message);

            try
            {
                var result = await _activitiesClient.GetLatestActivities(message.AccountId);

                return new GetAccountLatestActivitiesResponse
                {
                    Result = result
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not retrieve account latest activities successfully.");

                return new GetAccountLatestActivitiesResponse
                {
                    Result = new AggregatedActivitiesResult
                    {
                        Aggregates = new AggregatedActivityResult[0],
                        Total = 0
                    }
                };
            }
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