using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class AuthorisationResourceRepository : IAuthorisationResourceRepository
    {
        private const string TeamViewRoute = "accounts/{hashedaccountid}/teams/view";
        private const string TeamInvite = "accounts/{hashedaccountid}/teams/invite";
        private const string TeamReview = "accounts/{hashedaccountid}/teams/{email}/review";
        private const string TeamMemberRoleChange = "accounts/{hashedaccountid}/teams/{email}/role/change";

        public List<ResourceRoute> Get()
        {
            return new List<ResourceRoute>
            {
                new ResourceRoute { Name = "TeamViewRoute", Url = TeamViewRoute },
                new ResourceRoute { Name = "TeamInvite", Url = TeamInvite },
                new ResourceRoute { Name = "TeamReview", Url = TeamReview },
                new ResourceRoute { Name = "TeamMemberRoleChange", Url = TeamMemberRoleChange },
            };
           
        }
    }
}
