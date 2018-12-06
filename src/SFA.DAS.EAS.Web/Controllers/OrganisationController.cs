using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}/organisations")]
    public class OrganisationController : BaseController
    {
        public OrganisationController(
            IAuthenticationService owinWrapper,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
        }

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