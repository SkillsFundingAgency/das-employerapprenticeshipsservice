using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities;
using SFA.DAS.Activities.Client;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Queries.GetAccountActivities
{
    public class GetAccountActivitiesQueryHandler : IAsyncRequestHandler<GetAccountActivitiesQuery, GetAccountActivitiesResponse>
    {
        private readonly IActivitiesClient _activitiesClient;
        private readonly ILog _logger;
        private readonly IValidator<GetAccountActivitiesQuery> _validator;

        public GetAccountActivitiesQueryHandler(IActivitiesClient activitiesClient, ILog logger, IValidator<GetAccountActivitiesQuery> validator)
        {
            _activitiesClient = activitiesClient;
            _logger = logger;
            _validator = validator;
        }

        public async Task<GetAccountActivitiesResponse> Handle(GetAccountActivitiesQuery message)
        {
            ValidateMessage(message);

            try
            {
                var result = await _activitiesClient.GetActivities(new ActivitiesQuery
                {
                    AccountId = message.AccountId,
                    Take = message.Take,
                    From = message.From,
                    To = message.To,
                    Term = message.Term,
                    Category = message.Category,
                    Data = message.Data
                });

                return new GetAccountActivitiesResponse
                {
                    Result = result
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not retrieve account activities successfully.");

                return new GetAccountActivitiesResponse
                {
                    Result = new ActivitiesResult
                    {
                        Activities = new Activity[0],
                        Total = 0
                    }
                };
            }
        }

        private void ValidateMessage(GetAccountActivitiesQuery message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            if (result.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }
        }
    }
}