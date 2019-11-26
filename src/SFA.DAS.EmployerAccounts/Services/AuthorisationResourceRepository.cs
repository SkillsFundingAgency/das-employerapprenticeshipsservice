using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using System.Collections.Generic;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class AuthorisationResourceRepository : IAuthorisationResourceRepository
    {   
        public List<ResourceRoute> Get()
        {
            return new List<ResourceRoute>
            {
                new ResourceRoute { Name = nameof(AuthorizedTier2Route.TeamViewRoute), Url = AuthorizedTier2Route.TeamViewRoute },
                new ResourceRoute { Name = nameof(AuthorizedTier2Route.TeamInvite), Url = AuthorizedTier2Route.TeamInvite },
                new ResourceRoute { Name = nameof(AuthorizedTier2Route.TeamInviteComplete), Url = AuthorizedTier2Route.TeamInviteComplete},
                new ResourceRoute { Name = nameof(AuthorizedTier2Route.TeamMemberRemove), Url = AuthorizedTier2Route.TeamMemberRemove},
                new ResourceRoute { Name = nameof(AuthorizedTier2Route.TeamReview), Url = AuthorizedTier2Route.TeamReview },
                new ResourceRoute { Name = nameof(AuthorizedTier2Route.TeamMemberRoleChange), Url = AuthorizedTier2Route.TeamMemberRoleChange },
                new ResourceRoute { Name = nameof(AuthorizedTier2Route.TeamMemberInviteResend), Url = AuthorizedTier2Route.TeamMemberInviteResend },
                new ResourceRoute { Name = nameof(AuthorizedTier2Route.TeamMemberInviteCancel), Url = AuthorizedTier2Route.TeamMemberInviteCancel },
            };
           
        }
    }
}
