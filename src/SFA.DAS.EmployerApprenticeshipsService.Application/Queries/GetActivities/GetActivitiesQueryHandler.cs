using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities;
using SFA.DAS.Activities.Client;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Queries.GetActivities
{
    public class GetActivitiesQueryHandler : IAsyncRequestHandler<GetActivitiesQuery, GetActivitiesResponse>
    {
        private readonly CurrentUser _currentUser;
        private readonly IActivitiesClient _activitiesClient;
        private readonly ILog _logger;
        private readonly IMembershipRepository _membershipRepository;

        public GetActivitiesQueryHandler(
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

        public async Task<GetActivitiesResponse> Handle(GetActivitiesQuery message)
        {
            var membership = await _membershipRepository.GetCaller(message.HashedAccountId, _currentUser.ExternalUserId);

            if (membership == null)
            {
                throw new UnauthorizedAccessException();
            }

            GetActivitiesResponse activitiesResponse;

            try
            {
                var result = await _activitiesClient.GetActivities(new ActivitiesQuery
                {
                    AccountId = membership.AccountId,
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
            catch (Exception ex)
            {
                _logger.Error(ex, "Could not retrieve account activities successfully.");

                activitiesResponse = new GetActivitiesResponse
                {
                    Result = new ActivitiesResult
                    {
                        Activities = new List<Activity>(),
                        Total = 0
                    }
                };
            }

            return activitiesResponse;
        }
    }
}