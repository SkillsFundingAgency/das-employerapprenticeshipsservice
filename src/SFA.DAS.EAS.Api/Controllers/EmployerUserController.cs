using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/user/{userRef}")]
    public class EmployerUserController : ApiController
    {
        private readonly EmployerAccountsApiConfiguration _configuration;

        public EmployerUserController(EmployerAccountsApiConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("accounts", Name = "Accounts")]
        [ApiAuthorize(Roles = "ReadUserAccounts")]
        [HttpGet]
        public IHttpActionResult GetUserAccounts(string userRef)
        {
            return Redirect(_configuration.BaseUrl + $"/api/user/{userRef}/accounts");
        }
    }
}