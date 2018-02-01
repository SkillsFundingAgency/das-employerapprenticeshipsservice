using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Activities.Client;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.UserProfile;

namespace SFA.DAS.EAS.Application.Queries.GetActivities
{
    public class GetActivitiesQueryHandler : IAsyncRequestHandler<GetActivitiesQuery, GetActivitiesResponse>
    {
        private readonly CurrentUser _currentUser;
        private readonly IActivitiesClient _activitiesClient;
        private readonly IMembershipRepository _membershipRepository;

        public GetActivitiesQueryHandler(CurrentUser currentUser, IActivitiesClient activitiesClient, IMembershipRepository membershipRepository)
        {
            _currentUser = currentUser;
            _activitiesClient = activitiesClient;
            _membershipRepository = membershipRepository;
        }

        public async Task<GetActivitiesResponse> Handle(GetActivitiesQuery message)
        {
            var membership = await _membershipRepository.GetCaller(message.HashedAccountId, _currentUser.ExternalUserId);

            if (membership == null)
            {
                throw new UnauthorizedAccessException();
            }

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
    }
}