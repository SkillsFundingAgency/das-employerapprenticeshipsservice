using SFA.DAS.EmployerAccounts.Models.Account;
using System.Security.Claims;
using SFA.DAS.EmployerAccounts.Extensions;


namespace SFA.DAS.EmployerAccounts.Services;

public class AuthorisationResourceRepository : IAuthorisationResourceRepository
{
    private readonly IUserContext _userContext;

    private const string TeamViewRoute = "accounts/{hashedaccountid}/teams/view";
    private const string TeamInvite = "accounts/{hashedaccountid}/teams/invite";
    private const string TeamInviteComplete = "accounts/{hashedaccountid}/teams/invite/next";
    private const string TeamMemberRemove = "accounts/{hashedaccountid}/teams/{email}/remove";
    private const string TeamReview = "accounts/{hashedaccountid}/teams/{email}/review";
    private const string TeamMemberRoleChange = "accounts/{hashedaccountid}/teams/{email}/role/change";
    private const string TeamMemberInviteResend = "accounts/{hashedaccountid}/teams/resend";
    private const string TeamMemberInviteCancel = "accounts/{hashedaccountid}/teams/{invitationId}/cancel";
    private const string ErrorAccessDenied = "error/accessdenied/{hashedaccountid}";

    public AuthorisationResourceRepository(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public IEnumerable<AuthorizationResource> Get(ClaimsIdentity claimsIdentity)
    {
        if(_userContext.IsSupportConsoleUser())
        {
            return new List<AuthorizationResource>
            {
                new AuthorizationResource { Name = nameof(TeamViewRoute), Value = TeamViewRoute },
                new AuthorizationResource { Name = nameof(TeamInvite), Value = TeamInvite },
                new AuthorizationResource { Name = nameof(TeamInviteComplete), Value = TeamInviteComplete},
                new AuthorizationResource { Name = nameof(TeamMemberRemove), Value = TeamMemberRemove},
                new AuthorizationResource { Name = nameof(TeamReview), Value = TeamReview },
                new AuthorizationResource { Name = nameof(TeamMemberRoleChange), Value = TeamMemberRoleChange },
                new AuthorizationResource { Name = nameof(TeamMemberInviteResend), Value = TeamMemberInviteResend },
                new AuthorizationResource { Name = nameof(TeamMemberInviteCancel), Value = TeamMemberInviteCancel },
                new AuthorizationResource { Name = nameof(ErrorAccessDenied), Value = ErrorAccessDenied },
            };
        }
        return new List<AuthorizationResource>();
    }
}