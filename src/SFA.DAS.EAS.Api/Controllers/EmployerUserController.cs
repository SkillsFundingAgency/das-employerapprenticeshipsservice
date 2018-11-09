using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Account.Api.Extensions;
using SFA.DAS.EAS.Account.Api.Orchestrators;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/user/{userRef}")]
    public class EmployerUserController : ApiController
    {
        private readonly UsersOrchestrator _orchestrator;

        public EmployerUserController(UsersOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("accounts", Name = "Accounts")]
        [ApiAuthorize(Roles = "ReadUserAccounts")]
        [HttpGet]
        public IHttpActionResult GetUserAccounts(string userRef)
        {
            return Redirect(Url.EmployerAccountsApiAction(Request.RequestUri.PathAndQuery));
        }

        [Route("accounts2", Name = "Accounts2")]
        [ApiAuthorize(Roles = "ReadUserAccounts")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserAccountsLegacy(string userRef)
        {
            var result = await _orchestrator.GetUserAccounts(userRef);

            if (result.Status == HttpStatusCode.OK)
            {
                return Ok(result.Data);
            }

            //TODO: Handle unhappy paths.
            return Conflict();
        }
    }
}