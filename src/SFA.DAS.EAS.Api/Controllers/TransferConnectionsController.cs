using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Account.Api.Extensions;
using SFA.DAS.EAS.Application.Queries.GetTransferConnections;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accounts/{hashedAccountId}/transfers/connections")]
    public class TransferConnectionsController : ApiController
    {
        [Route]
        public IHttpActionResult GetTransferConnections([FromUri] GetTransferConnectionsQuery query)
        {
            return Redirect(Url.EmployerAccountsApiAction(Request.RequestUri.PathAndQuery));
        }
    }
}