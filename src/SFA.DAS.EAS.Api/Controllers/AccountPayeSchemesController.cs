using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/payeschemes")]
    public class AccountPayeSchemesController : ApiController
    {
        private readonly EmployerAccountsApiConfiguration _configuration;

        public AccountPayeSchemesController(EmployerAccountsApiConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("", Name = "GetPayeSchemes")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public IHttpActionResult GetPayeSchemes(string hashedAccountId)
        {
            return Redirect(_configuration.BaseUrl + $"/api/accounts/{hashedAccountId}/payeschemes");
        }

        [Route("{payeschemeref}", Name = "GetPayeScheme")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public IHttpActionResult GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            return Redirect($"{_configuration.BaseUrl}/api/accounts/{hashedAccountId}/payeschemes/{HttpUtility.UrlEncode(payeSchemeRef)}");
        }
    }
}