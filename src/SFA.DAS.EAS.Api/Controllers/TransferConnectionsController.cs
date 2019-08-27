using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accounts/{hashedAccountId}/transfers/connections")]
    public class TransferConnectionsController : ApiController
    {
        private readonly EmployerAccountsApiConfiguration _configuration;

        public TransferConnectionsController(EmployerAccountsApiConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route]
        public IHttpActionResult GetTransferConnections(string hashedAccountId)
        {
            return Redirect(_configuration.BaseUrl + $"/api/accounts/{hashedAccountId}/transfers/connections");
        }
    }
}