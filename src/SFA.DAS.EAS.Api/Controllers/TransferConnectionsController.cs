using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accounts/{hashedAccountId}/transfers/connections")]
    public class TransferConnectionsController : ApiController
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public TransferConnectionsController(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route]
        public IHttpActionResult GetTransferConnections(string hashedAccountId)
        {
            return Redirect(_configuration.EmployerAccountsApiBaseUrl + $"/api/accounts/{hashedAccountId}/transfers/connections");
        }
    }
}