using System;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/transfersconnections")]
    public class AccountTransferConnectionsController : ApiController
    {
        [Route("", Name = "GetTransferConnections")]
        [ApiAuthorize(Roles = "ReadUserAccounts")]
        [HttpGet]
        public IHttpActionResult GetTransferConnections(string hashedAccountId)
        {
            throw new NotImplementedException();
        }
    }
}