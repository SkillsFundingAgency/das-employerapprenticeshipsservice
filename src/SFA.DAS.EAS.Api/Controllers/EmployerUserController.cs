using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/user/{userRef}")]
    public class EmployerUserController : RedirectController
    {
        public EmployerUserController(EmployerApprenticeshipsServiceConfiguration configuration) : base(configuration)
        {
        }

        [Route("accounts", Name = "Accounts")]
        [ApiAuthorize(Roles = "ReadUserAccounts")]
        [HttpGet]
        public IHttpActionResult GetUserAccounts(string userRef)
        {
            return RedirectToEmployerAccountsApi();
        }
    }
}