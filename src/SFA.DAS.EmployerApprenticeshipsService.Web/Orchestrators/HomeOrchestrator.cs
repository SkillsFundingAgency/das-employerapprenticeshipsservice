using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserAccounts;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUserInvitations;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetUsers;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators
{
    public class HomeOrchestrator : IOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IOwinWrapper _owinWrapper;

        //Required for running tests
        public HomeOrchestrator()
        {
            
        }

        public HomeOrchestrator(IMediator mediator, IOwinWrapper owinWrapper)
        {
            _mediator = mediator;
            _owinWrapper = owinWrapper;
        }

        public virtual async Task<SignInUserViewModel> GetUsers()
        {
            var actual = await _mediator.SendAsync(new GetUsersQuery());

            return new SignInUserViewModel
            {
                AvailableUsers = actual.Users.Select(x =>
                                                new SignInUserModel
                                                {
                                                    Email = x.Email,
                                                    FirstName = x.FirstName,
                                                    LastName = x.LastName,
                                                    UserId = x.UserRef
                                                }).ToList()
            };
        }

        public virtual async Task<OrchestratorResponse< UserAccountsViewModel>> GetUserAccounts()
        {
            var userIdClaim =  _owinWrapper.GetClaimValue("sub");
            if (!string.IsNullOrEmpty(userIdClaim))
            {
                var userId =  userIdClaim;
                var getUserAccountsQueryResponse = await _mediator.SendAsync(new GetUserAccountsQuery {UserId = userId });
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
            return null;
        }
    }
}