using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EAS.Web.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{HashedAccountId}/organisations")]
    public class OrganisationController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("confirm")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Confirm()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/confirm"));
        }

        [HttpGet]
        [Route("nextStep")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> OrganisationAddedNextSteps()
        {
            return Redirect(Url.EmployerAccountsAction($"organisations/nextStep"));
        }

        [HttpGet]
        [Route("nextStepSearch")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> OrganisationAddedNextStepsSearch()
        {
            return Redirect(Url.EmployerAccountsAction($"organisations/nextStepSearch"));
        }


        [HttpPost]
        [Route("nextStep")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> GoToNextStep()
        {
            return Redirect(Url.EmployerAccountsAction("nextStep"));
        }

        [HttpGet]
        [Route("review")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Review()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/review"));
        }

        [HttpPost]
        [Route("review")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> ProcessReviewSelection()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/review"));
        }

        [HttpPost]
        [Route("PostUpdateSelection")]
        public Microsoft.AspNetCore.Mvc.ActionResult GoToPostUpdateSelection()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/PostUpdateSelection"));
        }
    }
}