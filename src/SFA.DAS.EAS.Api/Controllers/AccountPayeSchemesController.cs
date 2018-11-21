using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Account.Api.Extensions;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/payeschemes")]
    public class AccountPayeSchemesController : ApiController
    {
        [Route("", Name = "GetPayeSchemes")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public IHttpActionResult GetPayeSchemes(string hashedAccountId)
        {
            return Redirect(Url.EmployerAccountsApiAction(Request.RequestUri.PathAndQuery));
        }

        [Route("{payeschemeref}", Name = "GetPayeScheme")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public IHttpActionResult GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            return Redirect(Url.EmployerAccountsApiAction(Request.RequestUri.PathAndQuery));
        }
    }
}