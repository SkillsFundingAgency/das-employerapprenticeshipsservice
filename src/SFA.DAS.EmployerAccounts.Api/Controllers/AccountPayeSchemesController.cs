using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/payeschemes")]
    public class AccountPayeSchemesController : Microsoft.AspNetCore.Mvc.ControllerBase
    {
        private readonly AccountsOrchestrator _orchestrator;

        public AccountPayeSchemesController(AccountsOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [Route("{payeschemeref}", Name = "GetPayeScheme")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            var result = await _orchestrator.GetPayeScheme(hashedAccountId, HttpUtility.UrlDecode(payeSchemeRef));

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [Route("", Name = "GetPayeSchemes")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPayeSchemes(string hashedAccountId)
        {
            var result = await _orchestrator.GetPayeSchemesForAccount(hashedAccountId);


            if (result == null)
            {
                return NotFound();
            }

            return Ok(
                new ResourceList(
                    result
                        .Select(
                            pv => new Resource
                            {
                                Id = pv.Ref,
                                Href = Url.Route(
                                    "GetPayeScheme",
                                    new
                                    {
                                        hashedAccountId,
                                        payeSchemeRef = HttpUtility.UrlEncode(pv.Ref)
                                    })
                            }))
            );
        }
    }
}