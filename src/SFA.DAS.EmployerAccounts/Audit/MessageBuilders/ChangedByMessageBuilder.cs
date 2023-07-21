using Microsoft.AspNetCore.Http;
using SFA.DAS.EmployerAccounts.Audit.Types;
using SFA.DAS.EmployerAccounts.Infrastructure;

namespace SFA.DAS.EmployerAccounts.Audit.MessageBuilders;

public class ChangedByMessageBuilder : IAuditMessageBuilder
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChangedByMessageBuilder(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Build(AuditMessage message)
    {
        message.ChangedBy = new Actor();
        SetOriginIpAddess(message.ChangedBy);
        SetUserIdAndEmail(message.ChangedBy);
    }

    private void SetOriginIpAddess(Actor actor)
    {
        actor.OriginIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString() == "::1"
            ? "127.0.0.1"
            : _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
    }

    private void SetUserIdAndEmail(Actor actor)
    {
        var user = _httpContextAccessor.HttpContext.User;
        if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
        {
            return;
        }

        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type.Equals(EmployerClaims.IdamsUserIdClaimTypeIdentifier, System.StringComparison.CurrentCultureIgnoreCase));
        if (userIdClaim == null)
        {
            throw new InvalidContextException($"User does not have claim {EmployerClaims.IdamsUserIdClaimTypeIdentifier} to populate AuditMessage.ChangedBy.Id");
        }

        actor.Id = userIdClaim.Value;


        var userEmailClaim = user.Claims.FirstOrDefault(c => c.Type.Equals(EmployerClaims.IdamsUserEmailClaimTypeIdentifier, System.StringComparison.CurrentCultureIgnoreCase));
        if (userEmailClaim == null)
        {
            throw new InvalidContextException($"User does not have claim {EmployerClaims.IdamsUserEmailClaimTypeIdentifier} to populate AuditMessage.ChangedBy.EmailAddress");
        }

        actor.EmailAddress = userEmailClaim.Value;
    }
}
