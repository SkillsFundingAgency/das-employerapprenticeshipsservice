using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [Route("accounts/{HashedAccountId}/organisations")]
    public class OrganisationController : Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("confirm")]
        public async Task<ActionResult> Confirm()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/confirm"));
        }

        [HttpGet]
        [Route("nextStep")]
        public async Task<ActionResult> OrganisationAddedNextSteps()
        {
            return Redirect(Url.EmployerAccountsAction($"organisations/nextStep"));
        }

        [HttpGet]
        [Route("nextStepSearch")]
        public async Task<ActionResult> OrganisationAddedNextStepsSearch()
        {
            return Redirect(Url.EmployerAccountsAction($"organisations/nextStepSearch"));
        }


        [HttpPost]
        [Route("nextStep")]
        public async Task<ActionResult> GoToNextStep()
        {
            return Redirect(Url.EmployerAccountsAction("nextStep"));
        }

        [HttpGet]
        [Route("review")]
        public async Task<ActionResult> Review()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/review"));
        }

        [HttpPost]
        [Route("review")]
        public async Task<ActionResult> ProcessReviewSelection()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/review"));
        }

        [HttpPost]
        [Route("PostUpdateSelection")]
        public ActionResult GoToPostUpdateSelection()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/PostUpdateSelection"));
        }
    }
}