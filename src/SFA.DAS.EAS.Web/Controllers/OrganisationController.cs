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

        /// <summary>
        /// AML-2459: Unused entry point
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("nextStep")]
        public async Task<ActionResult> OrganisationAddedNextSteps()
        {
            return Redirect(Url.EmployerAccountsAction($"organisations/nextStep"));
        }

        /// <summary>
        /// AML-2459: Unused entry point
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("nextStepSearch")]
        public async Task<ActionResult> OrganisationAddedNextStepsSearch()
        {
            return Redirect(Url.EmployerAccountsAction($"organisations/nextStepSearch"));
        }


        /// <summary>
        /// AML-2459: Unused entry point
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("nextStep")]
        public async Task<ActionResult> GoToNextStep()
        {
            return Redirect(Url.EmployerAccountsAction("nextStep"));
        }

        /// <summary>
        /// AML-2459: Unused entry point
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("review")]
        public async Task<ActionResult> Review()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/review"));
        }

        /// <summary>
        /// AML-2459: Unused entry point
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("review")]
        public async Task<ActionResult> ProcessReviewSelection()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/review"));
        }

        /// <summary>
        /// AML-2459: Unused entry point
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("PostUpdateSelection")]
        public ActionResult GoToPostUpdateSelection()
        {
            return Redirect(Url.EmployerAccountsAction("organisations/PostUpdateSelection"));
        }
    }
}