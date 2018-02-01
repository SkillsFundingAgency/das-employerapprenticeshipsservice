using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Client;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Queries.GetLatestActivities
{
    public class GetLatestActivitiesQueryHandler : IRequestHandler<GetLatestActivitiesQuery, GetLatestActivitiesResponse>
    {
        private readonly CurrentUser _currentUser;
        private readonly IActivitiesClient _activitiesClient;
        private readonly ILog _logger;
        private readonly IMembershipRepository _membershipRepository;

        public GetLatestActivitiesQueryHandler(
            CurrentUser currentUser,
            IActivitiesClient activitiesClient,
            ILog logger,
            IMembershipRepository membershipRepository)
        {
            _currentUser = currentUser;
            _activitiesClient = activitiesClient;
            _logger = logger;
            _membershipRepository = membershipRepository;
        }

        public GetLatestActivitiesResponse Handle(GetLatestActivitiesQuery message)
        {
            var membership = Task.Run(async () => await _membershipRepository.GetCaller(message.HashedAccountId, _currentUser.ExternalUserId)).Result;

            if (membership == null)
            {
                throw new UnauthorizedAccessException();
            }

            GetLatestActivitiesResponse response;

            try
            {
                var result = Task.Run(async () => await _activitiesClient.GetLatestActivities(membership.AccountId)).Result;

                return new GetLatestActivitiesResponse
                {
                    Result = result
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to load latest activities.");

                response = new GetLatestActivitiesResponse
                {
                    Result = new AggregatedActivitiesResult
                    {
                        Aggregates = new List<AggregatedActivityResult>(),
                        Total = 0
                    }
                };
            }

            return response;
        }
    }
}