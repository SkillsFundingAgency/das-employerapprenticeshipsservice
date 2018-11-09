using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Account.Api.Extensions;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/user/{userRef}")]
    public class EmployerUserController : ApiController
    {
        [Route("accounts", Name = "Accounts")]
        [ApiAuthorize(Roles = "ReadUserAccounts")]
        [HttpGet]
        public IHttpActionResult GetUserAccounts(string userRef)
        {
            return Redirect(Url.EmployerAccountsApiAction(Request.RequestUri.PathAndQuery));
        }
    }
}