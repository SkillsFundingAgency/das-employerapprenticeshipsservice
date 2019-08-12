using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/user/{userRef}")]
    public class EmployerUserController : ApiController
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public EmployerUserController(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route("accounts", Name = "Accounts")]
        [ApiAuthorize(Roles = "ReadUserAccounts")]
        [HttpGet]
        public IHttpActionResult GetUserAccounts(string userRef)
        {
            return Redirect(_configuration.EmployerAccountsApiBaseUrl + $"api/user/{userRef}/accounts");
        }
    }
}