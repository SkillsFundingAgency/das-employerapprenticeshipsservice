using System;
using System.Threading.Tasks;
using System.Web.Mvc;

using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedAccountId}/apprentices/manage")]
    public class EmployerManageApprenticesController : BaseController
    {
        private readonly EmployerManageApprenticeshipsOrchestrator _orchestrator;

        public EmployerManageApprenticesController(
            EmployerManageApprenticeshipsOrchestrator orchestrator, 
            IOwinWrapper owinWrapper,
            IFeatureToggle featureToggle, 
            IUserWhiteList userWhiteList)
                : base(owinWrapper, featureToggle, userWhiteList)
        {
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));

            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("all")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> ListAll(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _orchestrator
                .GetApprenticeships(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _orchestrator
                .GetApprenticeship(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
            return View(model);
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/edit", Name = "EditApprovedApprentice")]
        public async Task<ActionResult> Edit(string hashedAccountId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _orchestrator
                .GetApprenticeshipForEdit(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        [Route("{hashedApprenticeshipId}/changes/confirm", Name = "ConfirmApprenticeChanges")]
        public async Task<ActionResult> ConfirmChanges(string hashedAccountId, string hashedApprenticeshipId, ApprenticeshipViewModel apprenticeship)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            // ToDo: Add overlapping errors to ModelState..
            if (!ModelState.IsValid)
                return await RedisplayEditApprenticeshipView(apprenticeship);

            var model = await _orchestrator.GetConfirmChangesModel(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"), apprenticeship);

            return View(model);
        }

        private async Task<ActionResult> RedisplayEditApprenticeshipView(ApprenticeshipViewModel apprenticeship)
        {
            var response = await _orchestrator.GetApprenticeshipForEdit(apprenticeship.HashedAccountId, apprenticeship.HashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));
            response.Data.Apprenticeship = apprenticeship;

            return View("Edit", response);
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/view", Name = "ViewPendingChanges")]
        public ActionResult ViewChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            return View();
        }

        private async Task<bool> IsUserRoleAuthorized(string hashedAccountId, params Role[] roles)
        {
            return await _orchestrator
                .AuthorizeRole(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), roles);
        }
    }
}