using System.Security.Claims;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EmployerAccounts.Audit;

public class AuditApiClientForSupportUser : IAuditApiClient
{
    private readonly IAuditApiClient _client;
    private readonly IUserContext _userContext;

    public AuditApiClientForSupportUser(IAuditApiClient client, IUserContext userContext)
    {
        _client = client;
        _userContext = userContext;
    }
    
    public Task Audit(AuditMessage message)
    {
        if (_userContext.IsSupportConsoleUser())
        {
            var impersonatedUser = _userContext.GetClaimValue(DasClaimTypes.Id);
            var impersonatedUserEmail = _userContext.GetClaimValue(DasClaimTypes.Email);
            message.ChangedBy.Id = _userContext.GetClaimValue(ClaimTypes.Upn); // support user user principal name
            message.ChangedBy.EmailAddress = _userContext.GetClaimValue(ClaimTypes.Email); // support user Email

            if (message.RelatedEntities == null)
            {
                message.RelatedEntities = new List<AuditEntity>();
            }

            message.RelatedEntities.Add(new AuditEntity { Type = "UserImpersonatedId", Id = impersonatedUser });
            message.RelatedEntities.Add(new AuditEntity { Type = "UserImpersonatedEmail", Id = impersonatedUserEmail });
        }

        return _client.Audit(message);
    }
}