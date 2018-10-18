using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Commands.UpsertRegisteredUser;
using SFA.DAS.EAS.Application.Queries.GetUserAccounts;
using SFA.DAS.EAS.Application.Queries.GetUserInvitations;
using SFA.DAS.EAS.Application.Queries.GetUsers;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Orchestrators
{
    public class HomeOrchestrator
    {
        private readonly IMediator _mediator;

        //Required for running tests
        public HomeOrchestrator()
        {

        }

        public HomeOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public virtual async Task<ViewModels.SignInUserViewModel> GetUsers()
        {
            var actual = await _mediator.SendAsync(new GetUsersQuery());

            return new ViewModels.SignInUserViewModel
            {
                AvailableUsers = actual.Users.Select(x =>
                                                new UserViewModel
                                                {
                                                    Id = x.Id,
                                                    UserRef = x.UserRef,
                                                    Email = x.Email,
                                                    FirstName = x.FirstName,
                                                    LastName = x.LastName,
                                                }).ToList()
            };
        }

        public virtual async Task<OrchestratorResponse<UserAccountsViewModel>> GetUserAccounts(string userId)
        {
            var getUserAccountsQueryResponse = await _mediator.SendAsync(new GetUserAccountsQuery
            {
                UserRef = userId
            });
            var getUserInvitationsResponse = await _mediator.SendAsync(new GetNumberOfUserInvitationsQuery
            {
                UserId = userId
            });
            return new OrchestratorResponse<UserAccountsViewModel>
            {
                Data = new UserAccountsViewModel
                {
                    Accounts = getUserAccountsQueryResponse.Accounts,
                    Invitations = getUserInvitationsResponse.NumberOfInvites
                }
            };
        }

        public virtual async Task SaveUpdatedIdentityAttributes(string userRef, string email, string firstName, string lastName)
        {
            await _mediator.SendAsync(new UpsertRegisteredUserCommand
            {
                EmailAddress = email,
                UserRef = userRef,
                LastName = lastName,
                FirstName = firstName
            });
        }
    }
}