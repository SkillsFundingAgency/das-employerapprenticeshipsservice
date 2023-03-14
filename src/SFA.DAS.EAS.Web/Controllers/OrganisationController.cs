using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [Route("accounts/{HashedAccountId}/organisations")]
    public class OrganisationController : Controller
    {
        public IConfiguration Configuration { get; set; }
        public OrganisationController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("confirm")]
        public async Task<ActionResult> Confirm()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/confirm", Configuration));
        }

        [HttpGet]
        [Route("nextStep")]
        public async Task<ActionResult> OrganisationAddedNextSteps()
        {
            return Redirect(Url.EmployerAccountsAction($"organisations/nextStep", Configuration));
        }

        [HttpGet]
        [Route("nextStepSearch")]
        public async Task<ActionResult> OrganisationAddedNextStepsSearch()
        {
            return Redirect(Url.EmployerAccountsAction($"organisations/nextStepSearch", Configuration));
        }


        [HttpPost]
        [Route("nextStep")]
        public async Task<ActionResult> GoToNextStep()
        {
            return Redirect(Url.EmployerAccountsAction("nextStep", Configuration));
        }

        [HttpGet]
        [Route("review")]
        public async Task<ActionResult> Review()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/review", Configuration));
        }

        [HttpPost]
        [Route("review")]
        public async Task<ActionResult> ProcessReviewSelection()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/review", Configuration));
        }

        [HttpPost]
        [Route("PostUpdateSelection")]
        public ActionResult GoToPostUpdateSelection()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/PostUpdateSelection", Configuration));
        }
    }
}