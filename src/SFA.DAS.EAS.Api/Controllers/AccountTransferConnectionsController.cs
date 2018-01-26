using System;
using System.Web.Http;
using SFA.DAS.EAS.Api.Attributes;

namespace SFA.DAS.EAS.Api.Controllers
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