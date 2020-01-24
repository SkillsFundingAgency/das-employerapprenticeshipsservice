using SFA.DAS.Audit.Client;
using SFA.DAS.Audit.Types;
using SFA.DAS.Authentication;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Extensions;
using SFA.DAS.EmployerUsers.WebClientComponents;
using System.IdentityModel.Claims;
using System.Collections.Generic;
using SFA.DAS.EmployerAccounts.Configuration;

namespace SFA.DAS.EmployerAccounts.Services
{
    public class AuditApiClientForSupportUser : IAuditApiClient
    {
        private readonly IAuditApiClient _client;
        private readonly IAuthenticationService _authenticationService;
        private readonly EmployerAccountsConfiguration _config;
        public AuditApiClientForSupportUser(IAuditApiClient client, IAuthenticationService authenticationService, EmployerAccountsConfiguration config)
        {
            _client = client;
            _authenticationService = authenticationService;
            _config = config;
        }
        public Task Audit(AuditMessage message)
        {
            if (_authenticationService.IsSupportConsoleUser(_config.SupportConsoleUsers))
            {
                var impersonatedUser = _authenticationService.GetClaimValue(DasClaimTypes.Id);
                var impersonatedUserEmail = _authenticationService.GetClaimValue(DasClaimTypes.Email);
                message.ChangedBy.Id =  _authenticationService.GetClaimValue(ClaimTypes.Upn); // support user user principal name
                message.ChangedBy.EmailAddress = _authenticationService.GetClaimValue(ClaimTypes.Email); // support user Email

                if(message.RelatedEntities == null)
                {
                    message.RelatedEntities = new List<Entity>();
                }

                message.RelatedEntities.Add(new Entity { Type = "UserImpersonatedId", Id = impersonatedUser});
                message.RelatedEntities.Add(new Entity { Type = "UserImpersonatedEmail", Id = impersonatedUserEmail });
            }
            
            return _client.Audit(message);
        }
    }
}
