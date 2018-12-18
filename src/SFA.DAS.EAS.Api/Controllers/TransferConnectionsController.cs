using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Application.Queries.GetTransferConnections;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accounts/{hashedAccountId}/transfers/connections")]
    public class TransferConnectionsController : RedirectController
    {
        public TransferConnectionsController(EmployerApprenticeshipsServiceConfiguration configuration) : base(configuration)
        {
        }

        [Route]
        public IHttpActionResult GetTransferConnections([FromUri] GetTransferConnectionsQuery query)
        {
            return RedirectToEmployerAccountsApi();
        }
    }
}